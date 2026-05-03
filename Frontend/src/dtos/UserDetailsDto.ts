class UserDetailsDto {
    id: string;
    gender: string;
    age: number;
    weight: number; 
    height: number;
    activityLevel: string;
    bmr: number;
    tdee: number;

    public constructor(id: string, gender: string, age: number, weight: number, height: number, activityLevel: string, bmr: number, tdee: number)
    {
        this.id = id;
        this.gender = gender;
        this.age = age;
        this.weight = weight;
        this.height = height;
        this.activityLevel = activityLevel;
        this.bmr = bmr;
        this.tdee = tdee;
    }

    static fromDomain(details: any, userId: string, activityLevelEnum: string): UserDetailsDto {
        return new UserDetailsDto(
            userId,
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