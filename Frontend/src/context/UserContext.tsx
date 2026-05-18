import { createContext, useContext, useState } from "react";
import type UserDetails from "../models/UserDetails";

interface UserContextType {
    userId: string | null;
    setUserId: (id: string | null) => void;
    userDetails: UserDetails | null;
    setUserDetails: (details: UserDetails | null) => void;
}

const UserContext = createContext<UserContextType | undefined>(undefined);

export const UserProvider = ({ children }: { children: React.ReactNode }) => {
    const [userId, setUserId] = useState<string | null>(null);
    const [userDetails, setUserDetails] = useState<UserDetails | null>(null);

    return (
        <UserContext.Provider value={{ userId, setUserId, userDetails, setUserDetails }}>
            {children}
        </UserContext.Provider>
    );
};

export const useUser = () => {
    const context = useContext(UserContext);
    if (!context) throw new Error("useUser must be used within a UserProvider");
    return context;
};
