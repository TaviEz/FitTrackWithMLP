class UserDetailsDto {
    gender: string;
    age: number;
    weight: number; 
    height: number;
    activityLevel: string;
    bmr: number;
    tdee: number;

    public constructor(gender: string, age: number, weight: number, height: number, activityLevel: string, bmr: number, tdee: number)
    {
        this.gender = gender;
        this.age = age;
        this.weight = weight;
        this.height = height;
        this.activityLevel = activityLevel;
        this.bmr = bmr;
        this.tdee = tdee;
    }

    static fromDomain(details: any, activityLevelEnum: string): UserDetailsDto {
        return new UserDetailsDto(
            details.gender,
            details.age,
            details.weight,
            details.height,
            activityLevelEnum,
            details.bmr,
            details.tdee
        );
    }
}

export default UserDetailsDto