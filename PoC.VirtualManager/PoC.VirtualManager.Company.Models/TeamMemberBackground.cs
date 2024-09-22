using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoC.VirtualManager.Company.Client.Models
{
    public class TeamMemberBackground
    {
        public PersonalInfo PersonalDetails { get; set; }
        public FamilyBackground Family { get; set; }
        public EducationBackground Education { get; set; }
        public WorkExperience WorkHistory { get; set; }
        public SkillsAndCertifications SkillsCertifications { get; set; }
        public HealthAndWellbeing Health { get; set; }
        public PsychologicalProfile Psychology { get; set; }
        public InterestsAndHobbies Hobbies { get; set; }
        public SocialConnections Social { get; set; }
    }

    public class PersonalInfo
    {
        public string FullName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string Nationality { get; set; }
        public string MaritalStatus { get; set; }
        public string Address { get; set; }
        public string ContactNumber { get; set; }
        public string Email { get; set; }
    }

    public class FamilyBackground
    {
        public string ParentsNames { get; set; }
        public string Siblings { get; set; }
        public string ChildhoodEnvironment { get; set; }
        public List<string> SignificantChildhoodEvents { get; set; }
        public string RelationshipWithParents { get; set; }
        public string RelationshipWithSiblings { get; set; }
    }

    public class EducationBackground
    {
        public List<EducationDetails> EducationHistory { get; set; }
        public string HighestDegree { get; set; }
        public string Major { get; set; }
        public string InstitutionsAttended { get; set; }
    }

    public class EducationDetails
    {
        public string InstitutionName { get; set; }
        public string Degree { get; set; }
        public string Major { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public double GPA { get; set; }
    }

    public class WorkExperience
    {
        public List<JobExperience> JobHistory { get; set; }
        public string CurrentPosition { get; set; }
        public string CurrentEmployer { get; set; }
        public DateTime DateJoined { get; set; }
        public string ReasonForLeavingLastJob { get; set; }
    }

    public class JobExperience
    {
        public string JobTitle { get; set; }
        public string CompanyName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Responsibilities { get; set; }
        public string Achievements { get; set; }
    }

    public class SkillsAndCertifications
    {
        public List<string> TechnicalSkills { get; set; }
        public List<string> SoftSkills { get; set; }
        public List<string> Certifications { get; set; }
        public List<string> LanguagesSpoken { get; set; }
    }

    public class HealthAndWellbeing
    {
        public string PhysicalHealthStatus { get; set; }
        public string MentalHealthStatus { get; set; }
        public List<string> KnownAllergies { get; set; }
        public List<string> ChronicConditions { get; set; }
        public string ExerciseHabits { get; set; }
        public string DietPreferences { get; set; }
    }

    public class PsychologicalProfile
    {
        public string PersonalityType { get; set; }
        public List<string> StressCopingMechanisms { get; set; }
        public List<string> PastTraumas { get; set; }
        public string GeneralOutlookOnLife { get; set; }
        public string CommunicationStyle { get; set; }
    }

    public class InterestsAndHobbies
    {
        public List<string> Hobbies { get; set; }
        public List<string> PersonalProjects { get; set; }
        public List<string> VolunteerActivities { get; set; }
        public string FavoriteBooks { get; set; }
        public string FavoriteMovies { get; set; }
    }

    public class SocialConnections
    {
        public List<string> CloseFriends { get; set; }
        public string SocialMediaPresence { get; set; }
        public string CommunityInvolvement { get; set; }
        public string RelationshipStatus { get; set; }
        public List<string> NetworkingGroups { get; set; }
    }
}
