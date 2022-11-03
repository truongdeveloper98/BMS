export interface AuthResponseDto {
    IsAuthSuccessful: boolean;
    ErrorMessage: string;
    Token: string;
    Is2StepVerificationRequired: boolean;
    Provider: string;
    IsNewUser: boolean;
}
