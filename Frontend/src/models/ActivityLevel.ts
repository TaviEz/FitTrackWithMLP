class ActivityLevel {
    label: string;
    description: string;
    multiplier: number;

    public constructor(label: string, description: string, multiplier: number)
    {
        this.label = label;
        this.description = description;
        this.multiplier = multiplier;
    }

    public static empty(): ActivityLevel {
        return new ActivityLevel("", "", 0);
    }
}

export default ActivityLevel;

