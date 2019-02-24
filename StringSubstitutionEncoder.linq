<Query Kind="Program">
  <Reference>&lt;RuntimeDirectory&gt;\System.Numerics.dll</Reference>
  <NuGetReference>Newtonsoft.Json</NuGetReference>
  <Namespace>Newtonsoft.Json</Namespace>
  <Namespace>System.Numerics</Namespace>
</Query>

void Main()
{
	var testPath = @"C:\Users\cadeh\Dropbox\Documents\Professional\Assessments\InterviewQuestions\AsynchronyLabs\TestData";
	var testFiles = Directory.EnumerateFiles(testPath);
	foreach (var file in testFiles.Where(f => f.Contains("input")))
	{
		var input = File.ReadAllText(file);
		var output = File.ReadAllText(file?.Replace("input", "output"));
		if (input != null)
		{
			var encoded = encode(input);
			$"{encoded.Equals(output)} {encoded} : {output}".Dump();
		}
	}
}

static string encode(string stringToEncode)
{
	IStringEncoder encoder = new StringSubstitutionEncoder(stringToEncode);
	return encoder.Encode();
}

public interface IStringEncoder
{
	string Encode();
}

public class StringSubstitutionEncoder : IStringEncoder
{ 
	private Queue<byte> _processingQueue;
	private Stack<byte> _digitStack;
	private StringBuilder _stringBuilder;
	private string _stringToEncode;
	
	public StringSubstitutionEncoder(string stringToEncode)
	{
		try
		{
			_stringToEncode = stringToEncode;
			_stringBuilder = new StringBuilder();
			_processingQueue = new Queue<byte>(ASCIIEncoding.ASCII.GetBytes(stringToEncode.ToLower()));
			_digitStack = new Stack<byte>();
		}
		catch (Exception e)
		{
			throw new Exception($"An error occurred trying to instantiate a new instance of {nameof(StringSubstitutionEncoder)}.");
		}
	}
	
	public string Encode()
	{
		do
		{
			while (CharTypeChecker.IsDigit(_processingQueue.Peek()))
			{
				_digitStack.Push(_processingQueue.Dequeue());
				if (_processingQueue.Count == 0) break;
			}
			if (_digitStack.Count > 0)
			{
				EncodeDigits();
				_digitStack.Clear();
			}
			if (_processingQueue.Count == 0) break;
			EncodeNonNumeric(_processingQueue.Dequeue());
		}
		while (_processingQueue.Count > 0);
		return _stringBuilder.ToString();
	}

	protected void EncodeNonNumeric(byte input)
	{
		if (CharTypeChecker.IsDigit(input))
		{
			throw new ArgumentException("Input must be the ASCII number for a non-numeric character.");
		}
		if (CharTypeChecker.IsConsonant(input)) EncodeConsonant(input);
		else if (CharTypeChecker.IsSpace(input)) EncodeSpace(input);
		else if (CharTypeChecker.IsVowel(input)) EncodeVowel(input);
		else if (CharTypeChecker.IsY(input)) EncodeY(input);
		else if (CharTypeChecker.IsSpace(input)) EncodeSpace(input);
		else _stringBuilder.Append((char)input);
	}

	public void EncodeVowel(byte input)
	{
		if (!StaticProperties.ASCIIMappings.Vowels.TryGetValue(input, out byte replacement))
		{
			throw new ArgumentException("Input must be the ASCII number for a vowel.");
		}
		_stringBuilder.Append((char)replacement);
	}

	public void EncodeConsonant(byte input)
	{
		if (!StaticProperties.ASCIIMappings.Consonants.Contains(input))
		{
			throw new ArgumentException("Input must be the ASCII number for a consonant.");
		}
		_stringBuilder.Append((char)(input - 1));
	}

	public void EncodeDigits()
	{
		while (_digitStack.Count > 0)
		{
			_stringBuilder.Append((char)(_digitStack.Pop()));
		}
	}

	public void EncodeY(byte input)
	{
		if (StaticProperties.ASCIIMappings.Y != input)
		{
			throw new ArgumentException("Input must be the ASCII number for y.");
		}
		_stringBuilder.Append((char)(StaticProperties.ASCIIMappings.Space));
	}

	public void EncodeSpace(byte input)
	{
		if (input != StaticProperties.ASCIIMappings.Space)
		{
			throw new ArgumentException("Input must be the ASCII number for a space.");
		}
		_stringBuilder.Append((char)(StaticProperties.ASCIIMappings.Y));
	}

	protected class CharTypeChecker
	{
		public static bool IsVowel(byte input)
		{
			return StaticProperties.ASCIIMappings.Vowels.ContainsKey(input);
		}

		public static bool IsConsonant(byte input)
		{
			return StaticProperties.ASCIIMappings.Consonants.Contains(input);
		}

		public static bool IsY(byte input)
		{
			return StaticProperties.ASCIIMappings.Y == input;
		}

		public static bool IsSpace(byte input)
		{
			return StaticProperties.ASCIIMappings.Space == input;
		}

		public static bool IsDigit(byte input)
		{
			return StaticProperties.ASCIIMappings.Digits.Contains(input);
		}
	}

	protected class StaticProperties
	{
		public class ASCIIMappings
		{
			//Map (lower-case) vowels to ASCII number representing their substitutions:
			public static Dictionary<byte, byte> Vowels = new Dictionary<byte, byte>
			{
				{ 97, 49 },
				{ 101, 50 },
				{ 105, 51 },
				{ 111, 52 },
				{ 117, 53 }
			};
			public static HashSet<byte> Consonants = new HashSet<byte>(new byte[] { 98, 99, 100, 102, 103, 104, 106, 107, 108, 109, 110, 112, 113, 114, 115, 116, 118, 119, 120, 122 });
			public static HashSet<byte> Digits = new HashSet<byte>(new byte[] { 48, 49, 50, 51, 52, 53, 54, 55, 56, 57 });
			public static byte Y = 121;
			public static byte Space = 32;
		}
	}
}
