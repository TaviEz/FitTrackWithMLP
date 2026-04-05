import { Card, CardActionArea, CardContent, Typography } from "@mui/material";
import { styled } from "@mui/material/styles";
import theme from "../theme";

const StyledCard = styled(Card)(() => ({
  backgroundColor: theme.palette.primary.light,
  color: theme.palette.primary.contrastText,
  borderRadius: "0.5rem",                 // subtler, matches input corners
  padding: "0.6rem 1.5rem",               // balanced padding
  textTransform: "none",                  // remove ALL CAPS
  fontWeight: 600,
  fontSize: "1rem",
  letterSpacing: "0.02em",
  width: 220,
  height: 160,
  boxShadow: "0 2px 6px rgba(0,0,0,0.15)", // soft shadow for depth
  transition: "all 0.2s ease-in-out",
  minWidth: "8rem",
  border: `1px solid ${theme.palette.divider}`,
  "&:hover": {
    boxShadow: "0 4px 12px rgba(0,0,0,0.15)",
    transform: "translateY(-2px)",
    cursor: "pointer",
  },
  "&:active": {
    transform: "translateY(0)",
    boxShadow: "0 2px 4px rgba(0,0,0,0.1)",
  },
}));

export function ThemedCard({
  title,
  description,
  onClick,
}: {
  title: string;
  description: string;
  onClick: () => void;
}) {
  return (
    <StyledCard>
      <CardActionArea onClick={onClick} sx={{ height: "100%", p: 2 }}>
        <CardContent>
          <Typography variant="h6" gutterBottom>
            {title}
          </Typography>
          <Typography variant="body2" color="text.secondary">
            {description}
          </Typography>
        </CardContent>
      </CardActionArea>
    </StyledCard>
  );
}
