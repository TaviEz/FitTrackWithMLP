import './App.css'
import { useEffect } from 'react'
import { BrowserRouter, Route, Routes, useLocation } from 'react-router-dom'
import Login from './components/authentication/LoginPage'
import SignInForm from './components/authentication/SignInPage'
import Onboarding from './components/onboarding/OnboardingPage'
import Dashboard from './components/Dashboard'
import TopNavBar from './components/shared/TopNavBar'
import { useUser } from './context/UserContext'
import { getLoggedUserId, getUserDetails } from './api/UserProfileService'
import UserDetailsDto from './dtos/UserDetails/UserDetailsDto'

const HIDDEN_NAV_ROUTES = ['/', '/signIn', '/onboarding'];

const AppLayout = () => {
  const { pathname } = useLocation();
  const showNav = !HIDDEN_NAV_ROUTES.includes(pathname);
  const { setUserId, setUserDetails } = useUser();

  useEffect(() => {
    if (!showNav) {
      return;
    }

    const rehydrate = async () => {
      const userId = await getLoggedUserId();
      if (!userId) {
        return;
      }

      setUserId(userId);

      const dto = await getUserDetails();
      if (dto) setUserDetails(UserDetailsDto.toDomain(dto));
    };

    rehydrate();
  }, [showNav, setUserId, setUserDetails]);

  return (
    <>
      {showNav && <TopNavBar />}
      <div style={{ paddingTop: showNav ? 20 : 0 }}>
        <Routes>
          <Route path="/" element={<Login />} />
          <Route path="/signIn" element={<SignInForm />} />
          <Route path="/onboarding" element={<Onboarding />} />
          <Route path="/dashboard" element={<Dashboard />} />
        </Routes>
      </div>
    </>
  );
};

const App = () => {
  return (
    <BrowserRouter>
      <AppLayout />
    </BrowserRouter>
  );
};

export default App
