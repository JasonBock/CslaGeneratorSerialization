using CslaGeneratorSerialization.Analysis.Descriptors;
using CslaGeneratorSerialization.Analysis.Diagnostics;
using Microsoft.CodeAnalysis;
using NUnit.Framework;
using System.Globalization;

namespace CslaGeneratorSerialization.Analysis.Tests.Descriptors;

internal static class BusinessObjectDoesNotHaveSerializationAttributeDescriptorTests
{
	[Test]
	public static void Create()
	{
		var descriptor = BusinessObjectDoesNotHaveSerializationAttributeDescriptor.Create();

		using (Assert.EnterMultipleScope())
		{
			Assert.That(descriptor.Id, Is.EqualTo(BusinessObjectDoesNotHaveSerializationAttributeDescriptor.Id));
			Assert.That(descriptor.Title.ToString(CultureInfo.CurrentCulture), Is.EqualTo(BusinessObjectDoesNotHaveSerializationAttributeDescriptor.Title));
			Assert.That(descriptor.MessageFormat.ToString(CultureInfo.CurrentCulture), Is.EqualTo(BusinessObjectDoesNotHaveSerializationAttributeDescriptor.Message));
			Assert.That(descriptor.DefaultSeverity, Is.EqualTo(DiagnosticSeverity.Error));
			Assert.That(descriptor.Category, Is.EqualTo(DiagnosticConstants.Usage));
			Assert.That(descriptor.HelpLinkUri, Is.EqualTo(HelpUrlBuilder.Build(
				BusinessObjectDoesNotHaveSerializationAttributeDescriptor.Id, BusinessObjectDoesNotHaveSerializationAttributeDescriptor.Title)));
		}
	}
}