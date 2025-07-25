export interface ResetPasswordRequestDTO {
  email: string;
  token: string;
  newPassword: string;
}
