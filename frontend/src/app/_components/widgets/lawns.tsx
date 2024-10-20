import React from 'react';

interface LawnItem {
  name: string;
  amount: string;
  interestRate: string;
  duration: string;
  progress: number;
}

const lawnItems: LawnItem[] = [
  { name: 'Real Estate', amount: '20.000.000,00€', interestRate: '3.37%', duration: '34 days', progress: 87 },
  { name: 'Equipment', amount: '500.000,00€', interestRate: '4.27%', duration: '48 months', progress: 50 },
  { name: 'Working Capital', amount: '30.000.000,00€', interestRate: '6.32%', duration: '9 months', progress: 63 },
  { name: 'Office equipment', amount: '500.000,00€', interestRate: '4.27%', duration: '220 months', progress: 50 },
];

const Lawns: React.FC = () => (
  <div className="bg-white shadow-sm rounded-lg p-6">
    <div className="flex justify-between items-center mb-4">
      <h2 className="text-lg font-bold">Lawns <span className="bg-yellow-400 text-white text-xs rounded-full px-2 py-1">4</span></h2>
      <button className="text-gray-500">•••</button>
    </div>
    <table className="w-full text-left">
      <tbody>
        {lawnItems.map((lawn, index) => (
          <tr key={index} className={index === lawnItems.length - 1 ? '' : 'border-b'}>
            <td className="py-2">{lawn.name}</td>
            <td className="py-2">{lawn.amount}</td>
            <td className="py-2">{lawn.interestRate}</td>
            <td className="py-2">{lawn.duration}</td>
            <td className="py-2">
              <div className="flex items-center">
                <div className="w-20 h-2 bg-gray-200 rounded-full mr-2">
                  <div
                    className={`h-2 rounded-full ${lawn.progress > 50 ? 'bg-green-500' : lawn.progress === 50 ? 'bg-yellow-500' : 'bg-red-500'}`}
                    style={{ width: `${lawn.progress}%` }}
                  ></div>
                </div>
                <span>{lawn.progress}%</span>
              </div>
            </td>
          </tr>
        ))}
      </tbody>
    </table>
  </div>
);

export default Lawns;
