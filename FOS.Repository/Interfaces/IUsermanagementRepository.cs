﻿using FOS.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FOS.Repository.Interfaces
{

    /// <summary>
    /// Repository for UserManagement.
    /// </summary>
    public interface IUsermanagementRepository
    {

        /// <summary>
        /// Get List of UserLevel Lookup.
        /// </summary>
        /// <returns>List of <see cref="Lookup"/></returns>
        public Task<List<Lookup>> GetUserLevelLookup(int? companyId, int? userId);



        /// <summary>
        /// Get List of Userdesignationlevel Lookup.
        /// </summary>
        /// <returns>List of <see cref="Lookup"/></returns>
        public Task<List<Lookup>> GetUserdesignationlevelLookup(int? companyId);



        /// <summary>
        /// Get the User Details.
        /// </summary>
        /// <returns>List of <see cref="InsertUserDetailsModel"/></returns>
        public Task<GetInsertUserDetailsModel> GetExistingUserDetails(int? companyId, int? userId);



        /// <summary>
        /// GetUserTranslander.
        /// </summary>
        /// <returns>List of <see cref="InsertUserDetailsModel"/></returns>
        public Task<List<GetUserTranslanderModel>> getUsertranslander(int? companyId, int? userId);


        /// <summary>
        /// GetUserReportingLevel.
        /// </summary>
        /// <returns>List of <see cref="ReportingLevel"/></returns>
        public Task<List<ReportingLevel>>getUserReportingLevel(int? companyId, int? userId,string? prefixText, int? lobId, int? locationId);


        /// <summary>
        /// Inserts a UserDetails.
        /// </summary>
        /// <param name="companyId">Company Id</param>
        /// <param name="User_ID">User Id</param>
        /// <param name="UserCode">UserCode</param>
        /// <param name="UserName">User Name </param>
        /// <param name="genderId">Gender Id</param>
        /// <param name="Password">@Password</param>
        /// <param name="mobileNumber">Mobile Number</param>
        /// <param name="EmergencycontactNumber">EmergencycontactNumber</param>
        /// <param name="DOJ">DOJ</param>
        /// <param name="Designation">Designation</param>
        /// <param name="UserLevelID">UserLevelID  </param>
        /// <param name="ReportingNextlevel">ReportingNextlevel  </param>
        /// <param name="User_Group">User_Group </param>
        /// <param name="EmailID">EmailID</param>
        /// <param name="Dateofbirth">Dateofbirth </param>
        /// <param name="FatherName">FatherName</param>
        /// <param name="MotherName">MotherName </param>
        /// <param name="SpouseName">SpouseName</param>
        /// <param name="Maritial_ID">Maritial_ID</param>
        /// <param name="Aadhar_Number">Aadhar_Number</param>
        /// <param name="PAN_Number">PAN_Number</param>
        /// <param name="Address"> Address</param>
        /// <param name="User_Imagepath">User_Imagepath </param>
        /// <param name="Is_Active">Is_Active </param>
        /// <param name="createdBy">Created By User Id.</param>
        /// <param name="errorCode">Error Code</param>
        /// <returns>Integer Value Indicating if the record got saved.</returns>

        public Task<int> InsertUserDetails(
                                           int companyId, int User_ID, string UserCode, string UserName,  int? genderId,
                                           string Password, DateTime? DOJ,  string mobileNumber, string? EmergencycontactNumber, int? Designation,
                                           int UserLevelID, int ReportingNextlevel, int? User_Group, string EmailID, DateTime? Dateofbirth,string FatherName,string MotherName
                                          ,string SpouseName,int Maritial_ID,string Aadhar_Number,string PAN_Number,string Address, string User_Imagepath,int Is_Active,
                                           int createdBy, int errorCode
                                                );
    }
}
