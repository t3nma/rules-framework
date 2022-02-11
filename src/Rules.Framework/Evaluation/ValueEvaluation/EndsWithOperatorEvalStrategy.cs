namespace Rules.Framework.Evaluation.ValueEvaluation
{
    using System;

    internal class EndsWithOperatorEvalStrategy : IOneToOneOperatorEvalStrategy
    {
        public bool Eval(object leftOperand, object rightOperand)
        {
            if (leftOperand is string && rightOperand is string)
            {
                string leftOperandAsString = leftOperand as string;
                string rightOperandAsString = rightOperand as string;

                return leftOperandAsString.EndsWith(rightOperandAsString);
            }

            throw new NotSupportedException($"Unsupported 'endswith' comparison between operands of type '{leftOperand?.GetType().FullName}' and '{rightOperand?.GetType().FullName}'.");
        }
    }
}