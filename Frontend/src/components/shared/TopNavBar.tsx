import { useNavigate, useLocation } from "react-router-dom";
import { AppBar, Toolbar, Tabs, Tab, Typography, Box } from "@mui/material";
import DashboardIcon from "@mui/icons-material/Dashboard";
import HistoryIcon from "@mui/icons-material/History";
import PersonIcon from "@mui/icons-material/Person";

const NAV_ITEMS = [
    { label: "Dashboard", path: "/dashboard", icon: <DashboardIcon fontSize="small" /> },
    { label: "History",   path: "/history",   icon: <HistoryIcon fontSize="small" /> },
    { label: "Profile",   path: "/profile",   icon: <PersonIcon fontSize="small" /> },
];

const TopNavBar = () => {
    const navigate = useNavigate();
    const { pathname } = useLocation();
    const activeIndex = NAV_ITEMS.findIndex((item) => item.path === pathname);
    const currentTab = activeIndex === -1 ? false : activeIndex;

    return (
        <AppBar position="fixed" elevation={1} sx={{ bgcolor: "primary.dark" }}>
            <Toolbar sx={{ justifyContent: "space-between", px: { xs: 2, sm: 4 } }}>
                <Typography
                    variant="h6"
                    fontWeight={700}
                    sx={{ color: "primary.contrastText", letterSpacing: 1 }}
                >
                    FitTrack
                </Typography>

                <Box>
                    <Tabs
                        value={currentTab}
                        onChange={(_, newValue) => navigate(NAV_ITEMS[newValue].path)}
                        textColor="inherit"
                        slotProps={{ indicator: { style: { backgroundColor: "#63ADF2" } } }}
                        sx={{ minHeight: 64 }}
                    >
                        {NAV_ITEMS.map((item) => (
                            <Tab
                                key={item.path}
                                label={item.label}
                                icon={item.icon}
                                iconPosition="start"
                                sx={{
                                    textTransform: "none",
                                    fontWeight: 600,
                                    minHeight: 64,
                                    color: "rgba(255,255,255,0.7)",
                                    "&.Mui-selected": { color: "#ffffff" },
                                }}
                            />
                        ))}
                    </Tabs>
                </Box>
            </Toolbar>
        </AppBar>
    );
};

export default TopNavBar;
