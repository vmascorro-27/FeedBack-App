using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using FeedBack_APP.Models.Entities;
using MySql.Data.EntityFramework;

namespace FeedBack_APP.Data
{
    [DbConfigurationType(typeof(MySqlEFConfiguration))]
    public class FeedbackDbContext : DbContext
    {
        public FeedbackDbContext(string connectionString)
            : base(connectionString)
        {
        }

        public DbSet<CatFormType> CatFormTypes { get; set; } = null!;
        public DbSet<CatPeriod> CatPeriods { get; set; } = null!;
        public DbSet<FeedbackFormD> FeedbackFormsD { get; set; } = null!;
        public DbSet<FeedbackFormDResponse> FeedbackFormsDResponses { get; set; } = null!;
        public DbSet<FeedbackFormG> FeedbackFormsG { get; set; } = null!;
        public DbSet<FeedbackResponse> FeedbackResponses { get; set; } = null!;
        public DbSet<FeedbackResponseD> FeedbackResponsesD { get; set; } = null!;
        public DbSet<FeedbackResponseG> FeedbackResponsesG { get; set; } = null!;
        public DbSet<PendingFeedback> PendingFeedback { get; set; } = null!;
        public DbSet<Role> Roles { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<CatFormType>().ToTable("cat_form_types");
            modelBuilder.Entity<CatFormType>().HasKey(item => item.Id);
            modelBuilder.Entity<CatFormType>().Property(item => item.Id).HasColumnName("id").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<CatFormType>().Property(item => item.FormType).HasColumnName("form_type").IsRequired().HasMaxLength(45);
            modelBuilder.Entity<CatFormType>().Property(item => item.Description).HasColumnName("description").IsOptional();
            modelBuilder.Entity<CatFormType>().Property(item => item.CreatedAt).HasColumnName("created_at").IsRequired();
            modelBuilder.Entity<CatFormType>().Property(item => item.ClientRemotie).HasColumnName("client_remotie").IsRequired();
            modelBuilder.Entity<CatFormType>().Property(item => item.IdPeriod).HasColumnName("id_period").IsOptional();

            modelBuilder.Entity<CatPeriod>().ToTable("cat_periods");
            modelBuilder.Entity<CatPeriod>().HasKey(item => item.Id);
            modelBuilder.Entity<CatPeriod>().Property(item => item.Id).HasColumnName("id").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<CatPeriod>().Property(item => item.Period).HasColumnName("period").IsRequired().HasMaxLength(45);
            modelBuilder.Entity<CatPeriod>().Property(item => item.Description).HasColumnName("description").IsOptional().HasMaxLength(225);
            modelBuilder.Entity<CatPeriod>().Property(item => item.CreatedAt).HasColumnName("created_at").IsOptional();

            modelBuilder.Entity<FeedbackFormD>().ToTable("feedback_forms_d");
            modelBuilder.Entity<FeedbackFormD>().HasKey(item => item.IdFormD);
            modelBuilder.Entity<FeedbackFormD>().Property(item => item.IdFormD).HasColumnName("id_form_d").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<FeedbackFormD>().Property(item => item.IdFormG).HasColumnName("id_form_g").IsRequired();
            modelBuilder.Entity<FeedbackFormD>().Property(item => item.Question).HasColumnName("question").IsRequired();
            modelBuilder.Entity<FeedbackFormD>().Property(item => item.WrittenOptionable).HasColumnName("written_optionable").IsRequired();
            modelBuilder.Entity<FeedbackFormD>().Property(item => item.Active).HasColumnName("active").IsRequired();

            modelBuilder.Entity<FeedbackFormDResponse>().ToTable("feedback_forms_d_responses");
            modelBuilder.Entity<FeedbackFormDResponse>().HasKey(item => item.IdFormsDResponse);
            modelBuilder.Entity<FeedbackFormDResponse>().Property(item => item.IdFormsDResponse).HasColumnName("id_forms_d_response").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<FeedbackFormDResponse>().Property(item => item.IdFormD).HasColumnName("id_form_d").IsRequired();
            modelBuilder.Entity<FeedbackFormDResponse>().Property(item => item.ResponseText).HasColumnName("reponse").IsRequired();
            modelBuilder.Entity<FeedbackFormDResponse>().Property(item => item.Active).HasColumnName("active").IsRequired();

            modelBuilder.Entity<FeedbackFormG>().ToTable("feedback_forms_g");
            modelBuilder.Entity<FeedbackFormG>().HasKey(item => item.IdFormG);
            modelBuilder.Entity<FeedbackFormG>().Property(item => item.IdFormG).HasColumnName("id_form_g").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<FeedbackFormG>().Property(item => item.IdFormType).HasColumnName("id_form_type").IsRequired();
            modelBuilder.Entity<FeedbackFormG>().Property(item => item.NameForm).HasColumnName("name_form").IsRequired().HasMaxLength(80);
            modelBuilder.Entity<FeedbackFormG>().Property(item => item.DateCreated).HasColumnName("date_created").IsRequired();
            modelBuilder.Entity<FeedbackFormG>().Property(item => item.IdUserCreated).HasColumnName("id_user_created").IsRequired();
            modelBuilder.Entity<FeedbackFormG>().Property(item => item.DateUpdated).HasColumnName("date_updated").IsOptional();
            modelBuilder.Entity<FeedbackFormG>().Property(item => item.IdUserUpdated).HasColumnName("id_user_updated").IsOptional();
            modelBuilder.Entity<FeedbackFormG>().Property(item => item.Active).HasColumnName("active").IsRequired();

            modelBuilder.Entity<FeedbackResponse>().ToTable("feedback_responses");
            modelBuilder.Entity<FeedbackResponse>().HasKey(item => item.Id);
            modelBuilder.Entity<FeedbackResponse>().Property(item => item.Id).HasColumnName("id").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<FeedbackResponse>().Property(item => item.FormTypeId).HasColumnName("form_type_id").IsRequired();
            modelBuilder.Entity<FeedbackResponse>().Property(item => item.Token).HasColumnName("token").IsRequired().HasMaxLength(100);
            modelBuilder.Entity<FeedbackResponse>().Property(item => item.SubmittedAt).HasColumnName("submitted_at").IsRequired();
            modelBuilder.Entity<FeedbackResponse>().Property(item => item.Data).HasColumnName("data").IsRequired();

            modelBuilder.Entity<FeedbackResponseD>().ToTable("feedback_responses_d");
            modelBuilder.Entity<FeedbackResponseD>().HasKey(item => item.IdFeedbackResponseD);
            modelBuilder.Entity<FeedbackResponseD>().Property(item => item.IdFeedbackResponseD).HasColumnName("id_feedback_response_d").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<FeedbackResponseD>().Property(item => item.IdFeedbackResponseG).HasColumnName("id_feedback_response_g").IsRequired();
            modelBuilder.Entity<FeedbackResponseD>().Property(item => item.Question).HasColumnName("question").IsRequired();
            modelBuilder.Entity<FeedbackResponseD>().Property(item => item.Response).HasColumnName("response").IsRequired();
            modelBuilder.Entity<FeedbackResponseD>().Property(item => item.Active).HasColumnName("active").IsRequired();

            modelBuilder.Entity<FeedbackResponseG>().ToTable("feedback_responses_g");
            modelBuilder.Entity<FeedbackResponseG>().HasKey(item => item.IdFeedbackResponseG);
            modelBuilder.Entity<FeedbackResponseG>().Property(item => item.IdFeedbackResponseG).HasColumnName("id_feedback_response_g").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<FeedbackResponseG>().Property(item => item.Token).HasColumnName("token").IsRequired().HasMaxLength(100);
            modelBuilder.Entity<FeedbackResponseG>().Property(item => item.NameForm).HasColumnName("name_form").IsRequired().HasMaxLength(80);
            modelBuilder.Entity<FeedbackResponseG>().Property(item => item.DateSurvey).HasColumnName("date_survey").IsRequired();
            modelBuilder.Entity<FeedbackResponseG>().Property(item => item.RemotieClient).HasColumnName("remotie_client").IsRequired();
            modelBuilder.Entity<FeedbackResponseG>().Property(item => item.SummaryAI).HasColumnName("summary_AI").IsOptional();
            modelBuilder.Entity<FeedbackResponseG>().Property(item => item.ScoreSurvey).HasColumnName("score_survey").IsOptional();
            modelBuilder.Entity<FeedbackResponseG>().Property(item => item.Active).HasColumnName("active").IsRequired();

            modelBuilder.Entity<PendingFeedback>().ToTable("pending_feedback");
            modelBuilder.Entity<PendingFeedback>().HasKey(item => item.Id);
            modelBuilder.Entity<PendingFeedback>().Property(item => item.Id).HasColumnName("id").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<PendingFeedback>().Property(item => item.CreatedAt).HasColumnName("created_at").IsRequired();
            modelBuilder.Entity<PendingFeedback>().Property(item => item.RemotieId).HasColumnName("remotie_id").IsRequired();
            modelBuilder.Entity<PendingFeedback>().Property(item => item.PeriodId).HasColumnName("period_id").IsRequired();
            modelBuilder.Entity<PendingFeedback>().Property(item => item.ClientSubmitted).HasColumnName("client_submitted").IsRequired();
            modelBuilder.Entity<PendingFeedback>().Property(item => item.RemotieSubmitted).HasColumnName("remotie_submitted").IsRequired();
            modelBuilder.Entity<PendingFeedback>().Property(item => item.Token).HasColumnName("token").IsRequired().HasMaxLength(100);
            modelBuilder.Entity<PendingFeedback>().Property(item => item.Attempt).HasColumnName("attempt").IsRequired();

            modelBuilder.Entity<Role>().ToTable("roles");
            modelBuilder.Entity<Role>().HasKey(item => item.IdRol);
            modelBuilder.Entity<Role>().Property(item => item.IdRol).HasColumnName("id_rol").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<Role>().Property(item => item.NombreRol).HasColumnName("nombre_rol").IsRequired().HasMaxLength(45);
            modelBuilder.Entity<Role>().Property(item => item.Activo).HasColumnName("activo").IsRequired();

            modelBuilder.Entity<User>().ToTable("users");
            modelBuilder.Entity<User>().HasKey(item => item.IdUser);
            modelBuilder.Entity<User>().Property(item => item.IdUser).HasColumnName("id_user").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<User>().Property(item => item.Name).HasColumnName("name").IsRequired().HasMaxLength(80);
            modelBuilder.Entity<User>().Property(item => item.Username).HasColumnName("user").IsRequired().HasMaxLength(80);
            modelBuilder.Entity<User>().Property(item => item.Pass).HasColumnName("pass").IsRequired();
            modelBuilder.Entity<User>().Property(item => item.DateCreated).HasColumnName("date_created").IsRequired();
            modelBuilder.Entity<User>().Property(item => item.IdRol).HasColumnName("id_rol").IsRequired();
            modelBuilder.Entity<User>().Property(item => item.Active).HasColumnName("active").IsRequired();

            modelBuilder.Entity<FeedbackFormD>()
                .HasRequired(item => item.FeedbackFormG)
                .WithMany(item => item.FeedbackFormsD)
                .HasForeignKey(item => item.IdFormG)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<CatFormType>()
                .HasOptional(item => item.Period)
                .WithMany(item => item.CatFormTypes)
                .HasForeignKey(item => item.IdPeriod)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<FeedbackFormDResponse>()
                .HasRequired(item => item.FeedbackFormD)
                .WithMany(item => item.FeedbackFormDResponses)
                .HasForeignKey(item => item.IdFormD)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<FeedbackFormG>()
                .HasRequired(item => item.FormType)
                .WithMany(item => item.FeedbackFormsG)
                .HasForeignKey(item => item.IdFormType)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<FeedbackFormG>()
                .HasRequired(item => item.UserCreated)
                .WithMany(item => item.FeedbackFormsCreated)
                .HasForeignKey(item => item.IdUserCreated)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<FeedbackFormG>()
                .HasOptional(item => item.UserUpdated!)
                .WithMany(item => item.FeedbackFormsUpdated)
                .HasForeignKey(item => item.IdUserUpdated)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<FeedbackResponseD>()
                .HasRequired(item => item.FeedbackResponseG)
                .WithMany(item => item.FeedbackResponsesD)
                .HasForeignKey(item => item.IdFeedbackResponseG)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasRequired(item => item.Role)
                .WithMany(item => item.Users)
                .HasForeignKey(item => item.IdRol)
                .WillCascadeOnDelete(false);
        }
    }
}
