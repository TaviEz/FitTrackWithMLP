namespace DailyPlanService.Services.MealOptimzer.Exceptions
{
    public class InfeasibleConstraintsException : Exception
    {
        public InfeasibleConstraintsException()
            : base("The optimization engine could not find a valid mathematical solution for the given macro targets.")
        {
        }

        public InfeasibleConstraintsException(string message)
            : base(message)
        {
        }

        public InfeasibleConstraintsException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
