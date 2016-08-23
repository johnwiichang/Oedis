#Oedis - A simple ORM Framework for Redis
.NET 下的 Redis ORM 框架

Oedis is an simple ORM based on StackExchange.Redis project, a high-performance open source Redis driver. Oedis is not a framework to replace Entity Framework just to improve its performance under these circumstances:

 1. Large log-like data
 2. Rank app with large records
 3. Data with TTL

If you are annoying about a project with EF meet these problems, I recommend Oedis to solve it until you really want to change the architecture.

##Install Oedis
Just typed:

	Install-Package Oedis

Oedis need StackExchange.Redis to connect Redis database.

##POCO for Oedis
You can use your Entity Framework POCO in Oedis. You should sign your Primary key with attribute `[Master]`, your Foreign key with `[Reference]`. If you want to ask Oedis skip a property, just mark it `[Except]`.
So, if you want to make a `Report` POCO, you can do it like:

	public class Report
	{
	    [Master]
	    [Key]
	    public Int32 Id { get; set;}
	
    	[Reference]
		[NotMapped]
    	public Guid Case_Id { get; set; }
	
    	[Except]
    	public virtual Case Case { get; set;}
	
	    public String Context { get; set; }
	}

You might find out that we seems don't support navigator property. Yep, I haven't made a implementation for it, and if you relly need, you can take part in.

##Set OedisContext
Just like the workflow as Entity Framework:

	class OedisContext : Oedis.OedisContext
	{
	    public OedisContext() : base() { }
	    public RedisSet<Report> Reports { get; set; }
	}


##Start Use
Get a context instance

	var OS = new OedisContext();

Inser a value

	OS.Reports.Add(new Report
	{
	    Id = 0,
	    Product = "EF",
	    Rid = Guid.NewGuid()
	});

Remove a value

	OS.Reports.Remove(
	    OS.Reports.Find(Guid.Parse(guidstr))
	);

Or you can remove lots of reports:

	OS.Reports.Remove(OS.Reports
	    .Where(x=>x.Rid==new Guid(guidstr)));

Oh... that will remove all reference related records, we'd better use:

	OS.Reports
	    .Clear(x=>x.Rid==new Guid(guidstr));

##Some Limitations You Need to Know

 - Oedis can't support multi-primary keys now, please consider use another method to solve this problem.
 - Oedis will insert your data into Redis, so your all values should be String compatible. Please try to write a ToString extension method to fit Oedis.
 - Current version of Odeis can only set one reference property. If you set multiple reference properties, Oedis will just map the first one in CLR.
 - The predicate lambda expressions is limited, please use simplified lambda in Oedis predicate lambda position. I will enhance it later.