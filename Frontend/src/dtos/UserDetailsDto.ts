class UserDetailsDto {
    id: string;
    gender: string;
    age: number;
    weight: number; 
    height: number;
    activityLevel: string;

    public constructor(id: string, gender: string, age: number, weight: number, height: number, activityLevel: string)
    {
        this.id = id;
        this.gender = gender;
        this.age = age;
        this.weight = weight;
        this.height = height;
        this.activityLevel = activityLevel;
    }
}

export default UserDetailsDto