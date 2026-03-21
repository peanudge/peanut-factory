import {
  createContext,
  useCallback,
  useContext,
  useState,
  type ReactNode,
} from "react";
import ToastContainer from "../components/ToastContainer";

type ToastSeverity = "success" | "info" | "warning" | "error";

export interface Toast {
  id: string;
  message: string;
  severity: ToastSeverity;
  duration: number | null;
}

interface ToastContextValue {
  toasts: Toast[];
  toast: (message: string, severity?: ToastSeverity, duration?: number | null) => void;
  dismiss: (id: string) => void;
}

const DEFAULT_DURATION: Record<ToastSeverity, number | null> = {
  success: 3000,
  info: 3000,
  warning: 5000,
  error: null,
};

const ToastContext = createContext<ToastContextValue | null>(null);

export function ToastProvider({ children }: { children: ReactNode }) {
  const [toasts, setToasts] = useState<Toast[]>([]);

  const dismiss = useCallback((id: string) => {
    setToasts((prev) => prev.filter((t) => t.id !== id));
  }, []);

  const toast = useCallback(
    (message: string, severity: ToastSeverity = "info", duration?: number | null) => {
      const id = crypto.randomUUID();
      const resolved = duration !== undefined ? duration : DEFAULT_DURATION[severity];

      setToasts((prev) => {
        const next = [...prev, { id, message, severity, duration: resolved }];
        return next.length > 5 ? next.slice(next.length - 5) : next;
      });

      if (resolved !== null && resolved > 0) {
        setTimeout(() => dismiss(id), resolved);
      }
    },
    [dismiss],
  );

  return (
    <ToastContext.Provider value={{ toasts, toast, dismiss }}>
      {children}
      <ToastContainer />
    </ToastContext.Provider>
  );
}

export function useToast(): ToastContextValue {
  const ctx = useContext(ToastContext);
  if (!ctx) throw new Error("useToast must be used inside <ToastProvider>");
  return ctx;
}
