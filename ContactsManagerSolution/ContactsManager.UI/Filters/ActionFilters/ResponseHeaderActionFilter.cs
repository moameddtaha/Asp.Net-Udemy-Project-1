using Microsoft.AspNetCore.Mvc.Filters;

namespace CRUDExample.Filters.ActionFilters
{
    public class ResponseHeaderFilterFactoryAttribute : Attribute, IFilterFactory
    {
        public bool IsReusable => false;
        private string? _key {  get; set; }
        private string? _value { get; set; }
        private int _order { get; set; }

        // Controller -> FilterFactory -> Filter
        public ResponseHeaderFilterFactoryAttribute(string key, string value, int order)
        {
            _key = key;
            _value = value;
            _order = order;
        }

        public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
        {
            var filter = serviceProvider.GetRequiredService<ResponseHeaderActionFilter>();
            filter.Key = _key;
            filter.Value = _value;
            filter.Order = _order;
            // Return filter object
            return filter;
        }
    }

    public class ResponseHeaderActionFilter : IAsyncActionFilter, IOrderedFilter
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public int Order { get; set; }
        private readonly ILogger<ResponseHeaderActionFilter> _logger;

        public ResponseHeaderActionFilter(ILogger<ResponseHeaderActionFilter> logger)
        {
            _logger = logger;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // Before
            _logger.LogInformation($"Before logic - {nameof(ResponseHeaderActionFilter)}");
            
            await next(); // Calls the subsequent filter or action method

            // After
            _logger.LogInformation($"After logic - {nameof(ResponseHeaderActionFilter)}");

            context.HttpContext.Response.Headers[Key] = Value;
        }
    }
}
