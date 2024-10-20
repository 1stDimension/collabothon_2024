"use client";

import Image from 'next/image';

export function NewDashboardButton() {
  return (
    <button className="h-[32px] flex items-center border border-[#E0E6E9] text-[#002F3F] font-bold px-4 py-2 rounded-[8px] hover:bg-[#F5F7F9]">
      <span className="mr-2">+</span>
      New Dashboard
    </button>
  );
}

export function EditDashboardButton() {
  return (
    <button className="h-[32px] flex items-center border border-[#E0E6E9] text-[#002F3F] font-bold px-4 py-2 rounded-[8px] hover:bg-[#F5F7F9]">
      <span className="mr-2"><Image src="/icon-pen.svg" alt="" width={15} height={15} /></span>
      Edit Dashboard
    </button>
  );
}

export function AiPoweredSearchButton() {
  return (
    <>
      <gen-search-widget
        configId="ccba4932-beba-45bc-b049-edfa09502889"
        location="eu"
        triggerId="searchWidgetTrigger">
      </gen-search-widget>
      <div className="w-[420px] h-[48px] bg-[#FFFFFF] flex items-center justify-between border border-[#E0E6E9] text-[#002F3F] font-bold px-4 py-2 rounded-[8px] cursor-pointer hover:bg-[#F5F7F9]">
        <div className="flex items-center gap-2">
          <Image src="/icon-search.svg" alt="" width={15} height={15} />
          <span
            id="searchWidgetTrigger"
            className="outline-none"
          >
            AI powered search...
          </span>
        </div>
        <Image src="/icon-microphone.svg" alt="" width={35} height={35} />
      </div>
      <script src="https://cloud.google.com/ai/gen-app-builder/client?hl=en_US" defer/>
    </>
  );
}

interface AiButtonProps {
  onClick: () => void;
};

export function AskAiAssistantButton({ onClick }: AiButtonProps) {
  return (
    // eslint-disable-next-line @typescript-eslint/no-unused-vars
    <button className="h-[48px] flex items-center bg-[#A1FFCE] text-[#002F3F] font-bold px-4 py-2 rounded-[8px] border border-[#53FF7F] hover:bg-[#53FF7F]" onClick={onClick}>
      <span className="mr-2">✨</span>
      Ask your AI assistant
    </button>
  );
}

