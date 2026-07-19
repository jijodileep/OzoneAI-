# FinancialYearSwitcher

Wire into the Ant Design layout header after E1.4 Vite scaffold:

```tsx
import { FinancialYearSwitcher } from "./components/FinancialYearSwitcher";

// in header:
<FinancialYearSwitcher />
```

Depends on API: `GET /v1/financial-years`, `POST /v1/session/financial-year`.
