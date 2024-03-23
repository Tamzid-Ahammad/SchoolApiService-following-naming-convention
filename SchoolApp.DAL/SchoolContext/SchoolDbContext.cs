using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;
using SchoolApp.Models.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;

namespace SchoolApp.DAL.SchoolContext
{
	public class SchoolDbContext : IdentityDbContext
	{

		public DbSet<Attendance> Attendances { get; set; }
		public DbSet<Standard> Standards { get; set; }
		public DbSet<Department> Departments { get; set; }
		public DbSet<ExamSchedule> ExamSchedules { get; set; }
		public DbSet<ExamSubject> ExamSubjects { get; set; }
		public DbSet<ExamType> ExamTypes { get; set; }
		public DbSet<Mark> Marks { get; set; }
		public DbSet<MarkEntry> MarkEntries { get; set; }
		public DbSet<Staff> Staff { get; set; }
		public DbSet<StaffExperience> staffExperiences { get; set; }
		public DbSet<StaffSalary> staffSalaries { get; set; }
		public DbSet<Student> Students { get; set; }
		public DbSet<Subject> Subjects { get; set; }
		public DbSet<FeeType> FeeTypes { get; set; }
		public DbSet<FeeStructure> FeeStructures { get; set; }
		public DbSet<FeePayment> FeePayments { get; set; }
		public DbSet<DueBalance> DueBalances { get; set; }
		public DbSet<FeePaymentDetail> FeePaymentDetails { get; set; }


		public SchoolDbContext(DbContextOptions<SchoolDbContext> options) : base(options)
		{

		}


		//This SaveChanges() method is implemented for inserting Computed column [NetSalary column from StaffSalary Table] into Database.
		public override int SaveChanges()
		{
			// Calculate NetSalary before saving changes
			foreach (var entry in ChangeTracker.Entries<StaffSalary>())
			{
				if (entry.State == EntityState.Added || entry.State == EntityState.Modified)
				{
					var staffSalary = entry.Entity;
					staffSalary.CalculateNetSalary();
				}
			}


			return base.SaveChanges();
		}








		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<IdentityUserLogin<string>>()
			.HasKey(u => new { u.UserId, u.LoginProvider, u.ProviderKey });

			modelBuilder.Entity<IdentityUserRole<string>>()
		.HasKey(r => new { r.UserId, r.RoleId });



			// Configure the foreign key constraint for dbsMark referencing dbsSubject

			modelBuilder.Entity<Mark>()
				.HasOne(m => m.Subject)
				.WithMany()
				.HasForeignKey(m => m.SubjectId)
				.OnDelete(DeleteBehavior.NoAction)
				; // Specify ON DELETE NO ACTION




			//    modelBuilder.Entity<StaffExperience>()
			//.Property(p => p.ServiceDuration)
			//.HasComputedColumnSql("DATEDIFF(year, JoiningDate, ISNULL(LeavingDate, GETDATE()))"); // Calculate duration in years



			modelBuilder.Entity<Subject>()
		.HasIndex(s => s.SubjectCode)
		.IsUnique();


			modelBuilder.Entity<Student>()
		.HasIndex(s => s.AdmissionNo)
		.IsUnique();


			modelBuilder.Entity<Student>()
		.HasIndex(s => s.EnrollmentNo)
		.IsUnique();


		}
	}
}
