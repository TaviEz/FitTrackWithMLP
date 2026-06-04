import { toast } from "react-toastify";

export const showError = (message: string) =>
    toast.error(message, { position: "bottom-right", autoClose: 4500 })

export const showWarning = (message: string) =>
    toast.warning(message, { position: "bottom-right", autoClose: 4500 })

export const showInfo = (message: string) =>
    toast.info(message, { position: "bottom-right", autoClose: 4500 })