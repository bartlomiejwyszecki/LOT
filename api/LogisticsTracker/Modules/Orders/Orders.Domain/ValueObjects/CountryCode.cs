namespace Orders.Domain.ValueObjects;

/// <summary>
/// Represents ISO 3166-1 alpha-3 country codes for standardized country representation.
/// </summary>
public enum CountryCode
{
    // Europe
    POL, // Poland
    DEU, // Germany
    FRA, // France
    GBR, // United Kingdom
    ITA, // Italy
    ESP, // Spain
    NLD, // Netherlands
    BEL, // Belgium
    AUT, // Austria
    CZE, // Czech Republic
    SVK, // Slovakia
    HUN, // Hungary
    ROU, // Romania
    BGR, // Bulgaria
    HRV, // Croatia
    SVN, // Slovenia
    SWE, // Sweden
    NOR, // Norway
    DNK, // Denmark
    FIN, // Finland
    GRC, // Greece
    PRT, // Portugal
    IRL, // Ireland
    LUX, // Luxembourg
    MLT, // Malta
    CYP, // Cyprus
    
    // Asia
    CHN, // China
    JPN, // Japan
    IND, // India
    KOR, // South Korea
    THA, // Thailand
    VNM, // Vietnam
    MYS, // Malaysia
    SGP, // Singapore
    IDN, // Indonesia
    PHL, // Philippines
    
    // Americas
    USA, // United States
    CAN, // Canada
    MEX, // Mexico
    BRA, // Brazil
    ARG, // Argentina
    CHL, // Chile
    
    // Africa
    ZAF, // South Africa
    EGY, // Egypt
    NGA, // Nigeria
    
    // Oceania
    AUS, // Australia
    NZL  // New Zealand
}
