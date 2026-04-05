import { Box, Paper, Stack, Typography } from "@mui/material"
import theme from "../../theme";
import { PrimaryButton } from "../../styledComponents/Buttons";

const ResultsForm = ({ bmr, tdee, handleNextSteps }: { bmr: number; tdee: number, handleNextSteps: () => void }) => {
    return (
        <Box 
            sx={{
                display: "flex", 
                flexDirection: "column", 
                justifyContent: "center", 
                alignItems: "center" 
            }}
        >
            <Stack spacing={2}>
                {bmr !== 0 && tdee !== 0 ? (
                    <>
                        <Typography sx={{...theme.typography.h6}}>Your Results</Typography>
                        <Paper variant="outlined" sx={{ p: 2, textAlign: "center" }}>
                            <Typography sx={{...theme.typography.body1}}>BMR</Typography>
                            <Typography sx={{...theme.typography.h4}}>{bmr}</Typography>
                            <Typography sx={{...theme.typography.body2}}>kcal/day</Typography>
                        </Paper>
                        <Paper variant="outlined" sx={{ p: 2, textAlign: "center" }}>
                            <Typography sx={{...theme.typography.body1}}>TDEE</Typography>
                            <Typography sx={{...theme.typography.h4}}>{tdee}</Typography>
                            <Typography sx={{...theme.typography.body2}}>kcal/day</Typography>
                        </Paper>
                        <PrimaryButton onClick={handleNextSteps}>
                            Next Steps
                        </PrimaryButton>
                    </>
                ) : (
                    <Typography sx={{...theme.typography.body1, textAlign: "center", maxWidth: 260}}>
                        Fill out your details and click <strong>Calculate</strong> to see your results here.
                    </Typography>
                )}
            </Stack>
        </Box>
    )
}

export default ResultsForm