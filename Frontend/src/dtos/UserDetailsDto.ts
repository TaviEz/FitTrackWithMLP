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
}

export default UserDetailsDto