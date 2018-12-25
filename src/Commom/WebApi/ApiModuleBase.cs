using AutoMapper;
using Common.Query;
using Common.Type;
using Nancy;
using Nancy.ModelBinding;
using Nancy.Responses;
using Nancy.Security;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Common.WebApi
{
    public abstract class ApiModuleBase : NancyModule
    {
        protected readonly IMapper Mapper;

        protected ApiModuleBase(bool requireAuthentication = true)
            : this(string.Empty, requireAuthentication) { }

        protected ApiModuleBase(string modulePath = "", bool requireAuthentication = true)
            : this(null, modulePath, requireAuthentication)
        { }

        protected ApiModuleBase(IMapper mapper, string modulePath = "", bool requireAuthentication = true)
             : base(modulePath)
        {
            Mapper = mapper;
            if (requireAuthentication)
            {
                this.RequiresAuthentication();
            }
        }

        protected FetchRequestHandler<TQuery, TResult> Fetch<TQuery, TResult>(Func<TQuery, Task<TResult>> fetch)
            where TQuery : IQuery, new() where TResult : class
        {
            var query = BindRequest<TQuery>();

            return new FetchRequestHandler<TQuery, TResult>(query, fetch, Negotiate, Request.Url, Mapper);
        }

        protected FetchRequestHandler<TQuery, TResult> FetchCollection<TQuery, TResult>(
            Func<TQuery, Task<PagedResult<TResult>>> fetch)
            where TQuery : IPagedQuery, new() where TResult : class
        {
            var query = BindRequest<TQuery>();

            return new FetchRequestHandler<TQuery, TResult>(query, fetch, Negotiate, Request.Url, Mapper);
        }

        protected T BindRequest<T>() where T : new()
        => Request.Body.Length == 0 && Request.Query == null
            ? new T()
            : this.Bind<T>();

        protected Response FromStream(Stream stream, string fileName, string contentType)
        {
            if (stream == null)
                return HttpStatusCode.NotFound;

            var response = new StreamResponse(() => stream, contentType);

            return response.AsAttachment(fileName);
        }
    }
}