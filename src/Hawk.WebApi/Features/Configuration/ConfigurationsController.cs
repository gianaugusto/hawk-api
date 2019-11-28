﻿namespace Hawk.WebApi.Features.Configuration
{
    using System.Threading.Tasks;

    using Hawk.Domain.Configuration;
    using Hawk.Domain.Shared.Exceptions;
    using Hawk.WebApi.Features.Shared;
    using Hawk.WebApi.Infrastructure;
    using Hawk.WebApi.Infrastructure.Authentication;
    using Hawk.WebApi.Infrastructure.ErrorHandling;
    using Hawk.WebApi.Infrastructure.ErrorHandling.TryModel;

    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;

    using static NewConfigurationModel;

    [ApiController]
    [ApiVersion("1")]
    [Route("configurations")]
    public class ConfigurationsController : BaseController
    {
        private readonly IGetConfigurationByDescription getConfigurationByDescription;
        private readonly IUpsertConfiguration upsertConfiguration;
        private readonly NewConfigurationModelValidator validator;

        public ConfigurationsController(
            IGetConfigurationByDescription getConfigurationByDescription,
            IUpsertConfiguration upsertConfiguration,
            IWebHostEnvironment environment)
            : this(getConfigurationByDescription, upsertConfiguration, new NewConfigurationModelValidator(), environment)
        {
            this.getConfigurationByDescription = getConfigurationByDescription;
            this.upsertConfiguration = upsertConfiguration;
        }

        internal ConfigurationsController(
            IGetConfigurationByDescription getConfigurationByDescription,
            IUpsertConfiguration upsertConfiguration,
            NewConfigurationModelValidator validator,
            IWebHostEnvironment environment)
            : base(environment)
        {
            this.getConfigurationByDescription = getConfigurationByDescription;
            this.upsertConfiguration = upsertConfiguration;
            this.validator = validator;
        }

        /// <summary>
        /// Get by description.
        /// </summary>
        /// <returns></returns>
        [HttpGet("{description}")]
        [ProducesResponseType(typeof(TryModel<ConfigurationModel>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetConfigurationByDescription([FromRoute] string description)
        {
            var entity = await this.getConfigurationByDescription.GetResult(this.GetUser(), description);

            return entity.Match(
                this.HandleError<ConfigurationModel>,
                configuration => this.Ok(new TryModel<ConfigurationModel>(new ConfigurationModel(configuration))));
        }

        /// <summary>
        /// Update.
        /// </summary>
        /// <param name="description"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut("{description}")]
        [ValidateSchema(typeof(NewConfigurationModel))]
        [ProducesResponseType(typeof(TryModel<ConfigurationModel>), 201)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(409)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateConfiguration(
            [FromRoute] string description,
            [FromBody] NewConfigurationModel request)
        {
            var validated = await this.validator.ValidateAsync(request);
            if (!validated.IsValid)
            {
                return this.HandleError<ConfigurationModel>(new InvalidObjectException("Invalid configuration.", validated));
            }

            var entity = await this.getConfigurationByDescription.GetResult(this.GetUser(), description);

            return await entity.Match(
                async _ =>
                {
                    var inserted = await this.upsertConfiguration.Execute(this.GetUser(), MapNewConfiguration(description, request));

                    return inserted.Match(
                        this.HandleError<ConfigurationModel>,
                        configuration => this.Created(new TryModel<ConfigurationModel>(new ConfigurationModel(configuration))));
                },
                async _ =>
                {
                    var updated = await this.upsertConfiguration.Execute(this.GetUser(), MapNewConfiguration(description, request));

                    return updated.Match(
                        this.HandleError<ConfigurationModel>,
                        configuration => this.NoContent());
                });
        }
    }
}