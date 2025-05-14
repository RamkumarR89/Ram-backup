using AutoMapper;
using System.Text.Json;
using ReportService.Domain.Entities;
using ReportService.Domain.DTOs;
using System.Collections.Generic;
using ReportService.Data;

namespace ReportService.Business.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Chat Session mappings
            CreateMap<ReportService.Domain.Entities.ChatSession, ChatSessionDto>()
                .ForMember(dest => dest.Messages, opt => opt.MapFrom(src => src.ChatMessages != null ? src.ChatMessages : new List<ReportService.Domain.Entities.ChatMessage>()))
                .ForMember(dest => dest.ReportName, opt => opt.MapFrom(src => src.ReportName))
                .ForMember(dest => dest.MeasuredValues, opt => opt.MapFrom(src => src.MeasuredValues));
            CreateMap<ChatSessionDto, ReportService.Domain.Entities.ChatSession>()
                .ForMember(dest => dest.ReportName, opt => opt.MapFrom(src => src.ReportName))
                .ForMember(dest => dest.MeasuredValues, opt => opt.MapFrom(src => src.MeasuredValues));

            // Chat Message mappings
            CreateMap<ReportService.Domain.Entities.ChatMessage, ChatMessageDto>();
            CreateMap<ChatMessageDto, ReportService.Domain.Entities.ChatMessage>();
            // Chat Session mappings
            CreateMap<ReportService.Domain.Entities.ChatSession, ChatSessionDto>()
                .ForMember(dest => dest.Messages, opt => opt.MapFrom(src => src.ChatMessages != null ? src.ChatMessages : new List<ReportService.Domain.Entities.ChatMessage>()))
                .ForMember(dest => dest.MeasuredValues, opt => opt.MapFrom(src => src.MeasuredValues));
            CreateMap<ChatSessionDto, ReportService.Domain.Entities.ChatSession>()
                .ForMember(dest => dest.MeasuredValues, opt => opt.MapFrom(src => src.MeasuredValues));

            // Chat Message mappings
            CreateMap<ReportService.Domain.Entities.ChatMessage, ChatMessageDto>();
            CreateMap<ChatMessageDto, ReportService.Domain.Entities.ChatMessage>();
            // Chart Configuration mappings
            CreateMap<ReportService.Domain.Entities.ChartConfiguration, ChartConfigurationDto>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => (ReportService.Domain.Enums.ChartTypeEnum)Enum.ToObject(typeof(ReportService.Domain.Enums.ChartTypeEnum), src.Type)))
                .ForMember(dest => dest.Options, opt => opt.Ignore())
                .ForMember(dest => dest.Filters, opt => opt.Ignore())
                .AfterMap((src, dest) => {
                    dest.Options = string.IsNullOrEmpty(src.OptionsJson) ? null : JsonSerializer.Deserialize<Dictionary<string, object>>(src.OptionsJson);
                    dest.Filters = string.IsNullOrEmpty(src.FiltersJson) ? null : JsonSerializer.Deserialize<Dictionary<string, object>>(src.FiltersJson);
                });
            CreateMap<ChartConfigurationDto, ReportService.Domain.Entities.ChartConfiguration>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => (int)src.Type))
                .ForMember(dest => dest.OptionsJson, opt => opt.Ignore())
                .ForMember(dest => dest.FiltersJson, opt => opt.Ignore())
                .AfterMap((src, dest) => {
                    dest.OptionsJson = src.Options != null ? JsonSerializer.Serialize(src.Options) : null;
                    dest.FiltersJson = src.Filters != null ? JsonSerializer.Serialize(src.Filters) : null;
                });

            // Session Workflow mappings
            CreateMap<ReportService.Domain.Entities.SessionWorkflow, SessionWorkflowStatusDto>();
            CreateMap<SessionWorkflowStatusDto, ReportService.Domain.Entities.SessionWorkflow>();

            // Measured Value mappings
            CreateMap<MeasuredValue, MeasuredValueDto>();
            CreateMap<MeasuredValueDto, MeasuredValue>();

            // Chart Type mappings
            CreateMap<ChartType, ChartTypeDto>();
        }
    }
} 