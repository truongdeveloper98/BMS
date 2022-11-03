export interface PositionVM {
    PositionName: string;
    PositionId: number;
}

export interface LevelVM {
    LevelName: string;
    Id: number;
}
  
export interface LanguageVM {
    LanguageName: string;
    Id: number;
}

export interface FrameworkVM {
    FrameworkName: string;
    Id: number;
}

export interface SalaryViewModel {
    Id:number | null,
    EffectiveDate: string | null,
    HourlySalary:number | null,
    User_Id:string[]
}

export interface SalaryInfo {
    Id:number | null,
    User_Id:string,    
    email: string,
    displayname: string,
    EffectiveDate: string | null,
    HourlySalary:number | null   
}