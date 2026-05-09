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
            details.weight,
            details.height,
            activityLevelEnum,
            details.bmr,
            details.tdee,
            details.goal
        );
    }
}

export default UserDetailsDto