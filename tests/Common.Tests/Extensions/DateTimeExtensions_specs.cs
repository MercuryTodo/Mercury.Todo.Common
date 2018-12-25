using Common.Extensions;
using Machine.Specifications;
using System;

namespace Common.Tests.Extensions
{
    [Subject("DateTimeExtensions ToTimestamp")]
    public class when_converting_date_time_to_js_timestamp
    {
        protected static DateTime DateTime;
        protected static long ExpectedValue = 946684800000;
        protected static long Result;

        private Establish context = () => DateTime = new DateTime(2000, 01, 01);

        private Because of = () => Result = DateTime.ToTimestamp();

        private It should_equal_expected_value = () => Result.ShouldEqual(ExpectedValue);
    }
}