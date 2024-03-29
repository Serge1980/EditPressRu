USE [EditPress]
GO
/****** Object:  StoredProcedure [dbo].[CategoriesForVendorsAdmin]    Script Date: 01.05.2020 7:58:22 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[CategoriesForVendorsAdmin]
	-- Add the parameters for the stored procedure here
	@vendorId int,
	@par nvarchar(10)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	
	Create table #DistCat
	(
		Id int,
		ProdId int,
		Name nvarchar(max),
		CpuPath nvarchar(max),
	)
    -- Insert statements for procedure here
	if @vendorId=0
	 
		insert into #DistCat (Id,ProdId,Name,CpuPath)
			
			select distinct a.Id, c.Id, a.Name,a.CpuPath  
			from Categories a 
			inner join ProdInCategory b on a.Id=b.CatId
			inner join Products c on c.Id=b.ProdId

	else 
	
		insert into #DistCat (Id,ProdId,Name,CpuPath)
			
			select distinct a.Id, c.Id, a.Name,a.CpuPath  
			from Categories a 
			inner join ProdInCategory b on a.Id=b.CatId
			inner join Products c on c.Id=b.ProdId
			where c.VendorId=@vendorId
	--select top 1 @CatId=Id from Categories 
	
	

	if @par='CatId'
		BEGIN

			declare @CatId int

			select distinct top 1 @CatId=Id 
				from #DistCat
				group by Id
				having count(ProdId)>1  

			select @CatId
			return
		End
	
	if @par='CatList'
		BEGIN
			select a.[Id]
				  ,a.[ParentId]
				  ,a.[CpuPath]
				  ,a.[Name]
				  ,a.[Publish]
				  ,'' as [ShortDesc]
				  ,'' as[Descript]
				  ,'' as[MetaTitle]
				  ,'' as[MetaDesc]
				  ,'' as[MetaKey]
				  ,'' as[Image]
				  ,a.[Ordering] 
				  from Categories a
				  inner JOIN (select distinct Name,CpuPath from  #DistCat) b on a.Name=b.Name and a.CpuPath=b.CpuPath

		End
	
	
END
