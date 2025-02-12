namespace Rules.Framework.Builder
{
    using System;
    using Rules.Framework.Evaluation;
    using Rules.Framework.Evaluation.ValueEvaluation;
    using Rules.Framework.Evaluation.ValueEvaluation.Dispatchers;
    using Rules.Framework.Validation;

    internal class ConfiguredRulesEngineBuilder<TContentType, TConditionType> : IConfiguredRulesEngineBuilder<TContentType, TConditionType>
    {
        private readonly IRulesDataSource<TContentType, TConditionType> rulesDataSource;
        private readonly RulesEngineOptions rulesEngineOptions;

        public ConfiguredRulesEngineBuilder(IRulesDataSource<TContentType, TConditionType> rulesDataSource)
        {
            this.rulesDataSource = rulesDataSource;
            this.rulesEngineOptions = RulesEngineOptions.NewWithDefaults();
        }

        public RulesEngine<TContentType, TConditionType> Build()
        {
            IOperatorEvalStrategyFactory operatorEvalStrategyFactory = new OperatorEvalStrategyFactory();

            IDataTypesConfigurationProvider dataTypesConfigurationProvider = new DataTypesConfigurationProvider(this.rulesEngineOptions);

            IConditionEvalDispatchProvider conditionEvalDispatchProvider = new ConditionEvalDispatchProvider(operatorEvalStrategyFactory, dataTypesConfigurationProvider);

            IDeferredEval deferredEval = new DeferredEval(conditionEvalDispatchProvider, this.rulesEngineOptions);

            IConditionsEvalEngine<TConditionType> conditionsEvalEngine = new ConditionsEvalEngine<TConditionType>(deferredEval);

            IConditionTypeExtractor<TContentType, TConditionType> conditionTypeExtractor = new ConditionTypeExtractor<TContentType, TConditionType>();

            ValidationProvider validationProvider = ValidationProvider.New()
                .MapValidatorFor(new SearchArgsValidator<TContentType, TConditionType>());

            return new RulesEngine<TContentType, TConditionType>(conditionsEvalEngine, this.rulesDataSource, validationProvider, this.rulesEngineOptions, conditionTypeExtractor);
        }

        public IConfiguredRulesEngineBuilder<TContentType, TConditionType> Configure(Action<RulesEngineOptions> configurationAction)
        {
            configurationAction.Invoke(this.rulesEngineOptions);
            RulesEngineOptionsValidator.EnsureValid(this.rulesEngineOptions);

            return this;
        }
    }
}