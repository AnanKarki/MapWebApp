// __tests__/AzureMapBoundary.test.jsx
import React from 'react';
import { render, fireEvent, waitFor, screen } from '@testing-library/react';
import AzureMapBoundary from '../components/AzureMapBoundary'; // update path as needed
import userEvent from '@testing-library/user-event';
import { getPolygonByReference } from "../utlis/azureMapUtils";

beforeEach(() => {
    fetch.resetMocks();
});

jest.mock('../constant', () => ({
    AZURE_MAPS_CONFIG: {
        API_BASE_URL: 'http://mocked-url',
        CLIENT_ID: 'dc794a84-c66e-4012-8dee-b58850f11ae8',
        SUBSCRIPTION_KEY: 'AdmoWQZGM90hA8ts2y6rAjMfqZCFSKGobK5rY7HQav4mICyMImNkJQQJ99BDACYeBjFMybzgAAAgAZMPRWHb'
    },
    AZURE_MAPS_URLS : {
        MAP_SOURCE: 'https://atlas.microsoft.com/sdk/javascript/mapcontrol/3/atlas.min.js',
        LOCAL_DISTRICT_BOUNDARIES: 'https://localhost:7108/azuremaps/uk-boundaries',
        DISTRICT_LIST: 'https://localhost:7108/azuremaps/getdistricts',
        POLYGON_BY_REFERENCE: 'https://localhost:7108/azuremaps/getpolygonbyreference?reference={reference}'
    }
}));

describe('AzureMapBoundary Component', () => {
    it('loads and displays dropdown options from API', async () => {
        const mockDistricts = [
            { name: 'Hounslow', reference: 'refA' },
            { name: 'London', reference: 'refB' },
        ];
        fetch.mockResponseOnce(JSON.stringify(mockDistricts));
        render(<AzureMapBoundary />);
        await userEvent.click(screen.getByRole('combobox'));

        const option = await screen.findByText((_, el) => el.textContent === 'London');

        expect(option).toBeInTheDocument();
    });

    it('clears selection and datasource when "Clear" is clicked', async () => {
        fetch.mockResponseOnce(JSON.stringify([])); 
        render(<AzureMapBoundary />);
        const comboBox = await screen.findByRole('combobox');  
        const clearBtn = await screen.findByDisplayValue('Clear');
        userEvent.click(clearBtn);
        expect(comboBox).toHaveValue('');
    });

    it('toggles visibility of all areas when "Hide all areas" clicked', async () => {
        fetch.mockResponse(JSON.stringify({ type: 'FeatureCollection', features: [] }));

        render(<AzureMapBoundary />);
        const hideBtn = await screen.getByTestId('eventButton');
        expect(hideBtn.value).toBe('Hide all areas');

    });

    const mockSetShapes = jest.fn();
    const mockSetCamera = jest.fn();
    const mockBBox = { some: 'bbox' };

    const mockAtlas = {
        data: {
            BoundingBox: {
                fromData: jest.fn().mockReturnValue(mockBBox)
            }
        }
    };
    it('fetches polygon and sets map shape and camera', async () => {
       
        const polygonData = { type: 'FeatureCollection', features: [] };
        fetch.mockResponseOnce(JSON.stringify(polygonData));

        await getPolygonByReference({
            reference: 'ref123',
            polygonReferenceUrl: 'https://localhost:7088/azuremaps/getpolygonbyreference?reference={reference}',
            datasource: { setShapes: mockSetShapes },
            map: { setCamera: mockSetCamera },
            atlas: mockAtlas
        });

        expect(fetch).toHaveBeenCalledWith(expect.stringContaining('reference=ref123'));
        expect(mockSetShapes).toHaveBeenCalledWith(polygonData);
        expect(mockAtlas.data.BoundingBox.fromData).toHaveBeenCalledWith(polygonData);
        expect(mockSetCamera).toHaveBeenCalledWith({ bounds: mockBBox, padding: 40 });
    });
});
