import { Box, Stack, Typography } from "@mui/material";
import { useState } from "react";
import { SecondaryButton } from "../../styledComponents/Buttons";
import { ThemedCard } from "../../styledComponents/Cards";

const DecisionFlow = ({ onComplete }: { onComplete: (choice: string, mode: string) => void }) => {
    const [step, setStep] = useState(1);
    const [choice, setChoice] = useState("");

    const handleChoice = (selected: string) => {
        setChoice(selected);
        setStep(2);
    };

    const handleMode = (mode: string) => {
        onComplete(choice, mode);
    };

    const handleBack = () => {
        setStep(step - 1);
        setChoice("");
    };

    return (
        <Box display="flex" flexDirection="column" alignItems="center" gap={4}>
            <Box textAlign="center">
                <Typography variant="h5" gutterBottom>
                    How would you like to begin your journey?
                </Typography>
                <Typography variant="body1" color="text.secondary">
                    Choose your starting point and how you'd like to track your progress.
                </Typography>
                <Typography variant="caption" display="block" mt={1}>
                    Step {step} of 2
                </Typography>
            </Box>

            {step === 1 && (
                <Stack direction="row" spacing={3}>
                    <ThemedCard
                        title="Meal Tracking"
                        description="Log your meals and monitor your nutrition."
                        onClick={() => handleChoice("meals")}
                    />
                    <ThemedCard
                        title="Workout Tracking"
                        description="Track your workouts and fitness progress."
                        onClick={() => handleChoice("workout")}
                    />
                </Stack>
            )}

            {step === 2 && (
                <>
                    <Stack direction="row" spacing={3}>
                        <ThemedCard
                            title={`AI Generated ${choice === "meals" ? "Meals" : "Workouts"}`}
                            description="Let AI create a personalized plan for you."
                            onClick={() => handleMode("AITrack")}
                        />
                        <ThemedCard
                            title={`Self Track ${choice}`}
                            description={`Manually track your own ${choice}.`}
                            onClick={() => handleMode("SelfTrack")}
                        />
                    </Stack>
                    <Box mt={2}>
                        <SecondaryButton onClick={handleBack}>Back</SecondaryButton>
                    </Box>
                </>
            )}
        </Box>
    );
};

export default DecisionFlow;
