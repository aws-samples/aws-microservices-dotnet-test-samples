using Autofac.Extras.FakeItEasy;
using AutoMapper;
using NUnit.Framework;

namespace Common.TestUtils;

public class TestBase<T> where T : Profile, new()
{
    private IMapper _mapper = null!;
    protected AutoFake AutoFake { get; private set; } = null!;

    [SetUp]
    public void Init()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile(new T()));

        _mapper = config.CreateMapper();

        AutoFake = new AutoFake();
        AutoFake.Provide(_mapper);
    }

    [TearDown]
    public void DisposeFakes()
    {
        AutoFake.Dispose();
    }
}