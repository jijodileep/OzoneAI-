/**
 * Header FY switcher for the React admin shell (E1.4+).
 * Calls POST /v1/session/financial-year and stores the returned JWT.
 */
import { useEffect, useState } from "react";
import { Select } from "antd";

type FinancialYear = {
  id: string;
  name: string;
  status: string;
  isDefault: boolean;
};

type SwitchResult = {
  financialYearId: string;
  name: string;
  isReadOnly: boolean;
  accessToken: string;
};

type Props = {
  apiBaseUrl?: string;
  onSwitched?: (result: SwitchResult) => void;
};

export function FinancialYearSwitcher({
  /** Empty = same-origin (Vite proxy in dev). */
  apiBaseUrl = "",
  onSwitched,
}: Props) {
  const [years, setYears] = useState<FinancialYear[]>([]);
  const [currentId, setCurrentId] = useState<string>();

  useEffect(() => {
    fetch(`${apiBaseUrl}/v1/financial-years`)
      .then((r) => r.json())
      .then((data: FinancialYear[]) => {
        setYears(data);
        const def = data.find((y) => y.isDefault) ?? data[0];
        if (def) setCurrentId(def.id);
      })
      .catch(console.error);
  }, [apiBaseUrl]);

  async function onChange(financialYearId: string) {
    const res = await fetch(`${apiBaseUrl}/v1/session/financial-year`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ financialYearId }),
    });
    if (!res.ok) throw new Error("FY switch failed");
    const result = (await res.json()) as SwitchResult;
    setCurrentId(result.financialYearId);
    localStorage.setItem("ozoneai.accessToken", result.accessToken);
    localStorage.setItem("ozoneai.financialYearId", result.financialYearId);
    onSwitched?.(result);
  }

  return (
    <Select
      style={{ minWidth: 160 }}
      value={currentId}
      onChange={onChange}
      options={years.map((y) => ({
        value: y.id,
        label: `${y.name}${y.status !== "Open" ? " (view)" : ""}`,
      }))}
      placeholder="Financial year"
    />
  );
}
