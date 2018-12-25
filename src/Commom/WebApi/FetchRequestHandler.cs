using AutoMapper;
using Common.Query;
using Common.Type;
using Nancy;
using Nancy.Responses.Negotiation;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Common.WebApi
{
    public class FetchRequestHandler<TQuery, TResult> where TQuery : IQuery, new() where TResult : class
    {
        private readonly ILogger _logger = Log.Logger;
        private readonly string PageParameter = "page";
        private readonly TQuery _query;
        private readonly Func<TQuery, Task<TResult>> _fetch;
        private readonly Func<TQuery, Task<PagedResult<TResult>>> _fetchCollection;
        private readonly Negotiator _negotiator;
        private readonly Url _url;
        private readonly IMapper _autoMapper;
        private Func<TResult, object> _mapper;

        public FetchRequestHandler(TQuery query,
            Func<TQuery, Task<TResult>> fetch,
            Negotiator negotiator,
            Url url,
            IMapper autoMapper = null)
        {
            _query = query;
            _fetch = fetch;
            _negotiator = negotiator;
            _url = url;
            _autoMapper = autoMapper;
        }

        public FetchRequestHandler(TQuery query,
            Func<TQuery, Task<PagedResult<TResult>>> fetchCollection,
            Negotiator negotiator,
            Url url,
            IMapper autoMapper = null)
        {
            _query = query;
            _fetchCollection = fetchCollection;
            _negotiator = negotiator;
            _url = url;
            _autoMapper = autoMapper;
        }

        public FetchRequestHandler<TQuery, TResult> MapTo(Func<TResult, object> mapper)
        {
            _mapper = mapper;

            return this;
        }

        public FetchRequestHandler<TQuery, TResult> MapTo<T>()
        {
            _mapper = x => _autoMapper.Map<T>(x);

            return this;
        }

        public async Task<Negotiator> HandleAsync()
            => _fetch == null ? await HandleCollectionAsync() : await HandleResultAsync();

        private async Task<Negotiator> HandleResultAsync()
        {
            var result = await _fetch(_query);
            if (result == null)
                return _negotiator.WithStatusCode(HttpStatusCode.NotFound);

            var value = result;
            if (value is Response)
                return _negotiator.WithModel(value);

            return FromResult(result);
        }

        private async Task<Negotiator> HandleCollectionAsync()
        {
            var result = await _fetchCollection(_query);

            return FromPagedResult(result);
        }

        private Negotiator FromResult(TResult result)
        {
            if (result == null)
            {
                _logger.Debug($"Result of {_query.GetType().Name} has no value {typeof(TResult).Name}");
                return _negotiator.WithStatusCode(HttpStatusCode.NotFound);
            }
            _logger.Debug($"Result of {_query.GetType().Name} contains {typeof(TResult).Name} object");
            var model = _mapper == null ? result : _mapper(result);

            return _negotiator.WithModel(model);
        }

        private Negotiator FromPagedResult(PagedResult<TResult> result)
        {
            if (result == null)
            {
                _logger.Debug($"Result of {_query.GetType().Name} has no value {typeof(TResult).Name}");
                return _negotiator.WithModel(new List<object>());
            }
            _logger.Debug($"Result of {_query.GetType().Name} contains {result.TotalResults} {typeof(TResult).Name} elements");
            var model = _mapper == null ? result.Items : result.Items.Select(x => _mapper(x));

            return _negotiator.WithModel(model)
                .WithHeader("Link", GetLinkHeader(result))
                .WithHeader("X-Total-Count", result.TotalResults.ToString());
        }

        private string GetLinkHeader(PagedResultBase result)
        {
            var first = GetPageLink(result.CurrentPage, 1);
            var last = GetPageLink(result.CurrentPage, (int)result.TotalPages);
            var prev = string.Empty;
            var next = string.Empty;
            if (result.CurrentPage > 1 && result.CurrentPage <= result.TotalPages)
                prev = GetPageLink(result.CurrentPage, result.CurrentPage - 1);
            if (result.CurrentPage < result.TotalPages)
                next = GetPageLink(result.CurrentPage, result.CurrentPage + 1);

            return $"{FormatLink(next, "next")}{FormatLink(last, "last")}" +
                   $"{FormatLink(first, "first")}{FormatLink(prev, "prev")}";
        }

        private string GetPageLink(int currentPage, int page)
        {
            var url = _url.ToString();
            var sign = string.IsNullOrWhiteSpace(_url.Query) ? "&" : "?";
            var pageArg = $"{PageParameter}={page}";
            var link = url.Contains($"{PageParameter}=")
                ? url.Replace($"{PageParameter}={currentPage}", pageArg)
                : url += $"{sign}{pageArg}";

            return link;
        }

        private string FormatLink(string url, string rel)
            => string.IsNullOrWhiteSpace(url) ? string.Empty : $"<{url}>; rel=\"{rel}\",";
    }
}