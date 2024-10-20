using CsvHelper.TypeConversion;
using CsvHelper;
using System.Globalization;
using SetlistManager.Models;
using System.Collections;

namespace SetlistManager.Services;

public static class CsvService
{   
    public static async Task<List<Song>> ReadCsvFile(Stream fileStream)
    {
        try
        {
            var reader = new StreamReader(fileStream);
            var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            var records = new List<Song>();
            await foreach (var record in csv.GetRecordsAsync<Song>())
            {
                records.Add(record);
            }
			return records.ToList();
        }
        catch (HeaderValidationException ex)
        {
            throw new ApplicationException("CSV file header is invalid.", ex);
        }
        catch (TypeConverterException ex)
        {
            throw new ApplicationException("CSV file contains invalid data format.", ex);
        }
        catch (Exception ex)
        {
            throw new ApplicationException("Error reading CSV file", ex);
        }
    }
}