export interface userDTO {
  id: string;
  displayname: string;
  username: string;
  email: string;
  first_name: string;
  last_name: string;
  birth_date: Date;
  start_date: Date;
  end_date: Date;
  address: string;
  avatar: string;
  position: string;
  department: number;
  phoneNumber: string;
  isDeleted: boolean;
  info: UserInfo;
}
export interface UserInfo {
  userid: string;
  level: string;
  team: string;
  department: string;
  position: string;
  ispending: boolean;
  pendingstart: Date;
  pendingend: Date;
  totalleaveday: number;
  avaiableleaveday: number;
  occupiedleaveday: number;
  typeid: number;
  effortfree: number;
  company: string;
  cvlink: string;
}
export interface RoleViewModel {
  Id: string;
  Name: string;
}
export interface UserTypeViewModel {
  Id: number;
  Name: string;
}
export interface changeUserStatus {
  isDeleted: boolean;
}
export interface userCreationDTO {
  id: string;
  username: string;
  email: string;
  first_name: string;
  last_name: string;
  birth_date: Date;
  start_date: string;
  end_date: Date;
  address: string;
  avatar: File;
  password: string;
  phone: string;
  role: string;
  department: string;
  type: number;
  totalleaveday: number;
  occupiedleaveday: number;
  team: string;
  level: string;
  ispending: boolean;
  position: string;
  pendingstart: Date;
  pendingend: Date;
  effortfree: number;
  company: string;
  cvlink: string;
}

export interface userVM {
  id: string;
  username: string;
  email: string;
  first_name: string;
  last_name: string;
  birth_date: Date;
  start_date: string;
  end_date: Date;
  address: string;
  avatar: File;
  password: string;
  phone: string;
  role: string;
  typeid: number;
  info: UserInfo;
}

export interface ProfileViewModel {
  First_Name: string;
  Last_Name: string;
  Birth_Date: string | null;
  Address: string;
  PhoneNumber: string;
}

export interface ChangePwViewModel {
  OldPassword: string;
  NewPassword: string;
}

export interface CompanyViewModel {
  CompanyId: number;
  CompanyName: string;
}
