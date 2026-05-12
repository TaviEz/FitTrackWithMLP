class UserDetailsDto {
    gender: string;
    age: number;
    weight: number; 
    height: number;
    activityLevel: string;
    bmr: number;
    tdee: number;
    goal: string;

    public constructor(gender: string, age: number, weight: number, height: number, activityLevel: string, bmr: number, tdee: number, goal: string)
    {
        this.gender = gender;
        this.age = age;
        this.weight = weight;
        this.height = height;
        this.activityLevel = activityLevel;
        this.bmr = bmr;
        this.tdee = tdee;
        this.goal = goal;
    }

    static fromDomain(details: any, activityLevelEnum: string): UserDetailsDto {
        return new UserDetailsDto(
            details.gender,
            details.age,
            Math.round(details.weight),
            Math.round(details.height),
            activityLevelEnum,
            Math.round(details.bmr),
            Math.round(details.tdee),
            details.goal
        );
    }
}

export default UserDetailsDto