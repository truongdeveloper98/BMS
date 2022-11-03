export interface Project {
  Id: number;
  Project_Name: string;
  ProjectType_Name: string;
  Project_Code: string;
  EndDate: string;
  StartDate: string;
  Description: string;
  Status: number;
  IsProjectPM: boolean;
}

export interface ProjectViewModel {
  Id: number | null;
  Project_Name: string;
  CustomerId: number;
  PartnerId: number;
  CustomerName: string;
  PartnerName: string;
  ProjectTypeId: number;
  StatusCoding: boolean;
  Project_Code: string;
  EndDate: string | null;
  StartDate: string | null;
  Description: string;
  Revenua: number;
  PM_Estimate: number | null;
  Brse_Estimate: number | null;
  Tester_Estimate: number | null;
  Comtor_Estimate: number | null;
  Developer_Estimate: number | null;
  UserPositions: UserPositions[];
  BacklogLink  : string;
}

export interface Users {
  Id: string;
  DisplayName: string;
  Avatar: string;
}

export interface UserPositions {
  PositionId: number;
  User_Id: string[];
}

export interface Positions {
  PositionId: number;
  PositionName: string;
}

export interface ProjectTypes {
  ProjectTypeId: number;
  ProjectTypeName: string;
}
