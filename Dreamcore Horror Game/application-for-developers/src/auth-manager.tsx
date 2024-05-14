export function isSignedIn(): boolean {
  return sessionStorage.getItem("refreshToken") ? true : false;
}

export function getCurrentLogin(): string {
  return sessionStorage.getItem("login") ?? "";
}

function getCurrentDeveloperAccessLevel(): string {
  return sessionStorage.getItem("developerAccessLevel") ?? "";
}

const lowAccessDeveloper = "Low Access Developer";
const mediumAccessDeveloper = "Medium Access Developer";
const fullAccessDeveloper = "Full Access Developer";

export function canViewDevelopmentTables(): boolean {
  const currentAccessLevel = getCurrentDeveloperAccessLevel();
  return currentAccessLevel === fullAccessDeveloper;
}

export function canCreate(): boolean {
  const currentAccessLevel = getCurrentDeveloperAccessLevel();
  return [mediumAccessDeveloper, fullAccessDeveloper].includes(currentAccessLevel);
}

export function canEdit(): boolean {
  const currentAccessLevel = getCurrentDeveloperAccessLevel();
  return [mediumAccessDeveloper, fullAccessDeveloper].includes(currentAccessLevel);
}

export function canDelete(): boolean {
  const currentAccessLevel = getCurrentDeveloperAccessLevel();
  return currentAccessLevel === fullAccessDeveloper;
}

export function signIn(login: string, refreshToken: string, developerAccessLevel: string) {
  sessionStorage.setItem("login", login);
  sessionStorage.setItem("refreshToken", refreshToken);
  sessionStorage.setItem("developerAccessLevel", developerAccessLevel);
}

export function signOut() {
  sessionStorage.removeItem("login");
  sessionStorage.removeItem("refreshToken");
  sessionStorage.removeItem("developerAccessLevel");
}
