export interface RecruitmentViewModel {
  Id: number | null;
  PositionId: number;
  FrameworkId: number;
  LanguageId: number;
  LevelId: number;
  Quantity: number;
  SalaryMin: number;
  SalaryMax: number;
  DatePublish: string | null;
  DateOnBroad: string | null;
  Description: string;
  PositionName: string;
  LanguageName: string;
  FrameworkName: string;
  Result: number;
  Status: number;
  Priority: number;
}
