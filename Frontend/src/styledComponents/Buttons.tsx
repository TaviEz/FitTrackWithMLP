import { Button } from "@mui/material";
import { styled } from "@mui/material/styles";
import theme from "../theme";

const StyledPrimaryButton = styled(Button)(() => ({
  backgroundColor: theme.palette.primary.main,
  color: theme.palette.primary.contrastText,
  borderRadius: "0.5rem",                 // subtler, matches input corners
  padding: "0.6rem 1.5rem",               // balanced padding
  textTransform: "none",                  // remove ALL CAPS
  fontWeight: 600,
  fontSize: "1rem",
  letterSpacing: "0.02em",
  boxShadow: "0 2px 6px rgba(0,0,0,0.15)", // soft shadow for depth
  transition: "all 0.2s ease-in-out",
  minWidth: "8rem",
  "&:hover": {
    backgroundColor: theme.palette.primary.dark,
    transform: "translateY(-1px)",        // subtle lift on hover
    boxShadow: "0 4px 10px rgba(0,0,0,0.2)",
  },
  "&:active": {
    transform: "translateY(0)",           // pressed back down
    boxShadow: "0 2px 4px rgba(0,0,0,0.15)",
  },
}));


const StyledSecondaryButton = styled(Button)(() => ({
  backgroundColor: theme.palette.primary.light,
  color: theme.palette.primary.contrastText,
  "&:hover": {
    backgroundColor: theme.palette.primary.main,
  }
}));

export function PrimaryButton(props: any) {
  return <StyledPrimaryButton {...props} />;
}

export function SecondaryButton(props: any) {
  return <StyledSecondaryButton {...props} />;
}