export interface GoalOption {
    label: string;
    value: string;
    description: string;
}

export const goalsData: GoalOption[] = [
    {
        label: "Lose Fat Aggressive",
        value: "LoseFatAggressive",
        description: "Aggressive caloric deficit for rapid fat loss"
    },
    {
        label: "Lean Cut Slow",
        value: "LeanCutSlow",
        description: "Moderate caloric deficit for gradual fat loss while preserving muscle"
    },
    {
        label: "Maintain Form Trained",
        value: "MaintainFormTrained",
        description: "Maintain current weight and muscle for trained individuals"
    },
    {
        label: "Maintain Form Healthy",
        value: "MaintainFormHealthy",
        description: "Maintain current weight for general health"
    },
    {
        label: "Lean Bulk",
        value: "LeanBulk",
        description: "Moderate caloric surplus for controlled muscle gain"
    },
    {
        label: "Power Bulk",
        value: "PowerBulk",
        description: "Aggressive caloric surplus for maximum muscle gain"
    }
];

export const defaultGoal = goalsData[2]; // MaintainFormTrained
