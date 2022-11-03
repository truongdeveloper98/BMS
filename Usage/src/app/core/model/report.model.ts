export interface WorkingReportVm {
  ReportId: number;
  ProjectId: number;
  ProjectName: string;
  PositionId: number;
  PositionName: string;
  Rate: number;
  Note: string;
  Time: number;
  Day: Date;
  WorkingType: number;
  DisplayName: string;
  UserId: string;
  Status: number;
  Description: string;
}
export interface PositionVm {
  PositionId: number;
  PositionName: string;
}
export interface ProjectVm {
  ProjectId: number;
  ProjectName: string;
}
export interface UserForReportVm {
  UserId: string;
  DisplayName: string;
  Visible: boolean;
}
export interface ProjectForReportVm {
  projectid: number;
  name: string;
}
export interface CreateReportVm {
  reportid: number;
  workingday: Date;
  workinghour: number;
  ratevalue: number;
  reporttype: number;
  note: string;
  status: number;
  description: string;
  projectid: number;
  positionid: number;
  workingtype: number;
  workingdays: Date[];
  startdate: Date;
  enddate: Date;
  thuhai: boolean;
  thuba: boolean;
  thutu: boolean;
  thunam: boolean;
  thusau: boolean;
}
export interface ReportOffVm {
  ReportOffId: number;
  OffDateStart: Date;
  OffDateEnd: Date;
  OffTypeId: number;
  Note: string;
  DisplayName: string;
  UserId: string;
  Status: number;
  Description: string;
}
export class ChangeStatusVm {
  status: number | undefined;
  description: string | undefined;
}
export interface createReportOffVm {
  reportoffid: number;
  offdatestart: Date;
  offdateend: Date;
  offtype: number;
  note: string;
  status: number;
  description: string;
}
