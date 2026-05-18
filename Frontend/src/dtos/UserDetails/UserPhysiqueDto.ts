class UserPhysiqueDto {
    tdee: number;
    weight: number;
    activityLevel: string;
    goalType: string;
    mealsComplexity: string;

    public constructor(tdee: number, weight: number, activityLevel: string, goalType: string, mealsComplexity: string = "Standard") {
        this.tdee = tdee;
        this.weight = weight;
        this.activityLevel = activityLevel;
        this.goalType = goalType;
        this.mealsComplexity = mealsComplexity;
    }
}

export default UserPhysiqueDto;