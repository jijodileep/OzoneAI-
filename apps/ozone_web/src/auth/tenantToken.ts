const KEY = 'ozoneai.tenantToken'

export function getTenantToken(): string | null {
  return localStorage.getItem(KEY)
}

export function setTenantToken(token: string): void {
  localStorage.setItem(KEY, token)
}

export function clearTenantToken(): void {
  localStorage.removeItem(KEY)
}

export function tenantAuthHeaders(): HeadersInit {
  const token = getTenantToken()
  return token ? { Authorization: `Bearer ${token}` } : {}
}
