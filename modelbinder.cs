using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ListModelBinderExample
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            // Add custom model binder for binding lists
            services.AddMvc(options =>
            {
                options.ModelBinderProviders.Insert(0, new ListModelBinderProvider());
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }

    public class ListModelBinder<T> : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var modelName = bindingContext.ModelName;
            var valueProviderResult = bindingContext.ValueProvider.GetValue(modelName);

            if (valueProviderResult == ValueProviderResult.None)
            {
                return Task.CompletedTask;
            }

            var value = valueProviderResult.FirstValue;
            if (string.IsNullOrEmpty(value))
            {
                return Task.CompletedTask;
            }

            try
            {
                var list = value.Split(',').Select(x => (T)Convert.ChangeType(x, typeof(T), CultureInfo.InvariantCulture)).ToList();
                bindingContext.Result = ModelBindingResult.Success(list);
            }
            catch (Exception ex)
            {
                bindingContext.ModelState.TryAddModelError(modelName, ex.Message);
            }

            return Task.CompletedTask;
        }
    }

    public class ListModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder? GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (!context.Metadata.IsEnumerableType)
            {
                return null;
            }

            var elementType = context.Metadata.ElementType;

            if (elementType == typeof(string))
            {
                return new ListModelBinder<string>();
            }
            else if (elementType == typeof(int))
            {
                return new ListModelBinder<int>();
            }
            else if (elementType == typeof(long))
            {
                return new ListModelBinder<long>();
            }

            return null;
        }
    }

    [ApiController]
    [Route("[controller]")]
    public class MyController : ControllerBase
    {
        [HttpGet("myroute/{list}")]
        public IActionResult MyAction(List<int> list)
        {
            // Do something with the list parameter
            return Ok(list);
        }
    }
}
