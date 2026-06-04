import './App.css'
import { useEffect } from 'react'
import { BrowserRouter, Route, Routes, useLocation } from 'react-router-dom'
import Login from './components/authentication/LoginPage'
import SignInForm from './components/authentication/SignInPage'
import Onboarding from './components/onboarding/OnboardingPage'
import Dashboard from './components/dashboard/Dashboard'
import ProfilePage from './components/profile/ProfilePage'
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
      if (!userId.success || !userId.data) {
        return;
      }

      setUserId(userId.data);

      const dto = await getUserDetails();
      if (dto.success && dto.data) setUserDetails(UserDetailsDto.toDomain(dto.data));
    };

    rehydrate();
  }, [showNav, setUserId, setUserDetails]);

  return (
    <>
      {showNav && <TopNavBar />}
      <div>
        <Routes>
          <Route path="/" element={<Login />} />
          <Route path="/signIn" element={<SignInForm />} />
          <Route path="/onboarding" element={<Onboarding />} />
          <Route path="/dashboard" element={<Dashboard />} />
          <Route path="/profile" element={<ProfilePage />} />
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
