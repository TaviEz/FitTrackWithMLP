import { createTheme } from '@mui/material/styles';

const theme = createTheme({
  palette: {
    primary: {
      main: '#304D6D',
      light: '#63ADF2',
      dark: '#1E2A3A',
      contrastText: '#FFFFFF'
    },
    text: {
      primary: '#1A1A1A',
      secondary: '#6F8FA8'
    },
    background: {
      default: '#FFFFFF',
      paper: '#F5F9FC',
    },
    divider: '#E0E0E0'
  },
  typography: {
    fontFamily: '"Roboto Flex", "Roboto", "Helvetica", "Arial", sans-serif',
    body1: {
      fontSize: '1.1rem',
      fontWeight: 500,
      lineHeight: 1.5,
      color: '#304D6D'
    },
    body2: {
      fontSize: '1rem',
      fontWeight: 500,
      lineHeight: 1.5,
      color: '#6F8FA8'
    },
    h2: {
      fontSize: '3.5rem',
      fontWeight: 600,
      color: '#304D6D'
    },
    h4: {
      fontSize: '2.3rem',
      fontWeight: 600,
      color: '#304D6D'
    },
    h5: {
      fontSize: '1.3rem',
      fontWeight: 600,
      lineHeight: 1.5,
      color: '#304D6D'
    },
    h6: {
      fontSize: '1rem',
      fontWeight: 600,
      lineHeight: 1.5,
      color: '#304D6D'
    }
  }
});

export default theme;
