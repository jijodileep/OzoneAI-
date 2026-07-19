const KEY = 'ozoneai.platformToken'

export function getPlatformToken(): string | null {
  return localStorage.getItem(KEY)
}

export function setPlatformToken(token: string): void {
  localStorage.setItem(KEY, token)
}

export function clearPlatformToken(): void {
  localStorage.removeItem(KEY)
}

export function platformAuthHeaders(): HeadersInit {
  const token = getPlatformToken()
  return token ? { Authorization: `Bearer ${token}` } : {}
}
