# مرحلة البناء (build)
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app

# نسخ ملفات المشروع
COPY . ./

# استعادة الحزم
RUN dotnet restore

# بناء المشروع
RUN dotnet publish -c Release -o out

# مرحلة التشغيل (runtime)
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS runtime
WORKDIR /app

# نسخ الملفات المبنية من المرحلة السابقة
COPY --from=build /app/out .

# تعريف البورت اللي هيشتغل عليه السيرفر داخل الكونتينر
EXPOSE 80
EXPOSE 443

# أمر التشغيل
ENTRYPOINT ["dotnet", "ExamApp.dll"]
