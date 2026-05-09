import { Box, Stack, Typography } from "@mui/material";
import { ThemedCard } from "../../styledComponents/Cards";

const DecisionFlow = ({ onComplete }: { onComplete: (mode: string) => void }) => {
    const handleMode = (mode: string) => {
        onComplete(mode);
    };

    return (
        <Box display="flex" flexDirection="column" alignItems="center" gap={4}>
            <Box textAlign="center">
                <Typography variant="h5" gutterBottom>
                    How would you like to begin your journey?
                </Typography>
                <Typography variant="body1" color="text.secondary">
                    Choose how you'd like to track your meal progress.
                </Typography>
            </Box>

            <Stack direction="row" spacing={3}>
                <ThemedCard
                    title="AI Generated Meals"
                    description="Let AI create a personalized meal plan for you."
                    onClick={() => handleMode("AITrack")}
                />
                <ThemedCard
                    title="Self Track Meals"
                    description="Manually track your own meals."
                    onClick={() => handleMode("SelfTrack")}
                />
            </Stack>
        </Box>
    );
};

export default DecisionFlow;
